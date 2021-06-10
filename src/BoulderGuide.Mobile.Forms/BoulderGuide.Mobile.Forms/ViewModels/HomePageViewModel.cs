using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.UI;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class HomePageViewModel : ViewModelBase {

      private readonly IDataService dataService;
      private readonly IPermissions permissions;
      private readonly IActivityIndicationService activityIndicationService;

      public ICommand ReloadCommand { get; }
      public ObservableCollection<AreaInfo> AreaInfos { get; } = new ObservableCollection<AreaInfo>();
      public AreaInfo SelectedAreaInfo { get; set; }
      public HomePageViewModel(
         IDataService dataService,
         IPermissions permissions,
         IActivityIndicationService activityIndicationService) {

         this.dataService = dataService;
         this.permissions = permissions;
         this.activityIndicationService = activityIndicationService;

         ReloadCommand = new Command(async (f) => await Reload((bool) f));

         Title = Strings.ClimbingAreas;
      }

      public override void Initialize(INavigationParameters parameters) {
         base.Initialize(parameters);

         ReloadCommand.Execute(false);

         Task.Run(async () => await InitializeAsync());
      }

      public void OnSelectedAreaInfoChanged() {
         NavigateAsync(
            SelectedAreaInfo.Name,
            $"/MainPage/NavigationPage/{nameof(AreaDetailsPage)}",
            AreaDetailsPageViewModel.InitializeParameters(SelectedAreaInfo),
            Icons.MaterialIconFont.Terrain);
      }
      private async Task InitializeAsync() {
         var status = await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
         if (status != PermissionStatus.Granted) {
            status = await permissions.RequestAsync<Permissions.LocationWhenInUse>();
         }
      }

      private async Task Reload(bool force) {
         var status = await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
         if (status != PermissionStatus.Granted) {
            status = await permissions.RequestAsync<Permissions.LocationWhenInUse>();
         }

         await activityIndicationService.StartLoadingAsync();
         AreaInfos.Clear();

         IEnumerable<AreaInfo> areas = await dataService.GetIndexAreas(force);

         foreach (var area in areas ?? Enumerable.Empty<AreaInfo>()) {
            AreaInfos.Add(area);
         }
         await activityIndicationService.FinishLoadingAsync();
      }
   }
}
