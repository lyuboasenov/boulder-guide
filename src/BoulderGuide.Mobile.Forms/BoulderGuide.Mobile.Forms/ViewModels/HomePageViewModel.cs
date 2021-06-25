using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.Errors;
using BoulderGuide.Mobile.Forms.Services.Location;
using BoulderGuide.Mobile.Forms.Services.UI;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class HomePageViewModel : ViewModelBase {

      private readonly IDataService dataService;
      private readonly IPermissions permissions;
      private readonly IActivityIndicationService activityIndicationService;
      private readonly ILocationService locationService;

      public ICommand ReloadCommand { get; }
      public ObservableCollection<AreaInfo> AreaInfos { get; } = new ObservableCollection<AreaInfo>();
      public AreaInfo SelectedAreaInfo { get; set; }
      public HomePageViewModel(
         IDataService dataService,
         IPermissions permissions,
         IActivityIndicationService activityIndicationService,
         ILocationService locationService) {

         this.dataService = dataService;
         this.permissions = permissions;
         this.activityIndicationService = activityIndicationService;
         this.locationService = locationService;

         ReloadCommand = new AsyncCommand<bool>(Reload);
      }

      public void OnSelectedAreaInfoChanged() {
         RunAsync(NavigateToAreaAsync);
      }

      private async Task NavigateToAreaAsync() {
         await NavigateAsync(
            SelectedAreaInfo.Name,
            $"/MainPage/NavigationPage/{nameof(AreaPage)}",
            AreaPageViewModel.InitializeParameters(SelectedAreaInfo),
            Icons.MaterialIconFont.Terrain);
      }

      public override void Initialize(INavigationParameters parameters) {
         base.Initialize(parameters);

         ReloadCommand.Execute(false);
      }

      private async Task Reload(bool force) {
         try {
            var status = await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) {
               await permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            using (activityIndicationService.StartLoading()) {
               AreaInfos.Clear();

               IEnumerable<AreaInfo> areas = await dataService.GetIndexAreas(force);

               foreach (var area in areas ?? Enumerable.Empty<AreaInfo>()) {
                  AreaInfos.Add(area);
               }
            }

            locationService.Run();

         } catch (Exception ex) {
            await HandleOperationExceptionAsync (ex, Strings.UnableToReloadClimbingRegions);
         }
      }
   }
}
