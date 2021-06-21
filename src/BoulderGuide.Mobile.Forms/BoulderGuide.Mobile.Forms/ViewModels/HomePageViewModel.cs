using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using BoulderGuide.Mobile.Forms.Services.Errors;
using BoulderGuide.Mobile.Forms.Services.UI;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
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
      private readonly IErrorService errorService;

      public ICommand ReloadCommand { get; }
      public ObservableCollection<AreaInfo> AreaInfos { get; } = new ObservableCollection<AreaInfo>();
      public AreaInfo SelectedAreaInfo { get; set; }
      public HomePageViewModel(
         IDataService dataService,
         IPermissions permissions,
         IActivityIndicationService activityIndicationService,
         IErrorService errorService) {

         this.dataService = dataService;
         this.permissions = permissions;
         this.activityIndicationService = activityIndicationService;
         this.errorService = errorService;

         ReloadCommand = new Command(async (f) => await Reload((bool) f));

         Title = Strings.ClimbingAreas;
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
      }
   }
}
