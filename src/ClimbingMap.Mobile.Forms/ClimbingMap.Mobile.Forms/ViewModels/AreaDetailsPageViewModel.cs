using ClimbingMap.Domain.Entities;
using ClimbingMap.Mobile.Forms.Services.Data;
using ClimbingMap.Mobile.Forms.Services.Maps;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace ClimbingMap.Mobile.Forms.ViewModels {
   public class AreaDetailsPageViewModel : ViewModelBase {

      private static Stack stack = new Stack();

      private readonly IDataService dataService;
      private readonly IConnectivity connectivity;
      private readonly IMapService mapService;
      private readonly IGeolocation geolocation;
      private readonly IPermissions permissions;
      private bool runLocationTracker = false;

      public ICommand DownloadCommand { get; }
      public bool IsLoading { get; set; }
      public Area Area { get; set; }
      public AreaInfo Info { get; set; }
      public AreaInfo SelectedAreaInfo { get; set; }
      public RouteInfo SelectedRouteInfo { get; set; }
      public Mapsui.Map Map { get; set; }
      public Mapsui.UI.Objects.MyLocationLayer MyLocationLayer { get; set; }

      public AreaDetailsPageViewModel(
         INavigationService navigationService,
         IDataService dataService,
         IConnectivity connectivity,
         IDialogService dialogService,
         IMapService mapService,
         IGeolocation geolocation,
         IPermissions permissions) :
         base (navigationService, dialogService) {
         this.dataService = dataService;
         this.connectivity = connectivity;
         this.mapService = mapService;
         this.geolocation = geolocation;
         this.permissions = permissions;

         DownloadCommand = new Command(async () => await Download());
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         if (parameters.TryGetValue<AreaInfo>(ParameterKeys.AreaInfo, out AreaInfo info)) {
            stack.Push(info);
         } else {
            stack.Pop();
            if (stack.Count == 0) {
               NavigationService.GoBackToRootAsync();
               return;
            } else {
               info = stack.Peek() as AreaInfo;
            }
         }

         Task.Run(async () => await InitializeAsync(info));
      }

      public override void OnNavigatedFrom(INavigationParameters parameters) {
         base.OnNavigatedFrom(parameters);
         runLocationTracker = false;
      }

      public void OnSelectedAreaInfoChanged() {
         NavigationParameters parameters = new NavigationParameters();
         parameters.Add(AreaDetailsPageViewModel.ParameterKeys.AreaInfo, SelectedAreaInfo);

         NavigationService.NavigateAsync($"{AreaDetailsPageViewModel.View}", parameters);
      }

      public void OnSelectedRouteInfoChanged() {
         NavigationParameters parameters = new NavigationParameters();
         parameters.Add(RoutePageViewModel.ParameterKeys.RouteInfo, SelectedRouteInfo);

         NavigationService.NavigateAsync($"{RoutePageViewModel.View}", parameters);
      }

      public const string View = "AreaDetailsPage";

      public static class ParameterKeys {
         public const string AreaInfo = "AreaInfo";
      }

      private async Task InitializeAsync(AreaInfo info) {
         IsLoading = true;

         Info = info;

         try {
            if (Info.IsOffline) {
               Area = await dataService.GetOfflineArea(Info);
            } else if (connectivity.NetworkAccess == NetworkAccess.Internet) {
               Area = await dataService.GetArea(Info);
            } else {
               var dialogParams = new DialogParameters();
               dialogParams.Add(
                  DialogPageViewModel.ParameterKeys.Message,
                  Strings.UnableToDownloadArea);
               dialogParams.Add(DialogPageViewModel.ParameterKeys.Severity, DialogPageViewModel.Severity.Error);

               await DialogService.ShowDialogAsync("Unable to download", dialogParams);
            }

            Map = mapService.GetMap(Area, Info);
            if (await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted) {
               RunLocationTracker();
            }
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
         IsLoading = false;
      }

      private void RunLocationTracker() {
         Device.StartTimer(TimeSpan.FromSeconds(30), () => {
            Task.Run(async () => {
               var currentLocation =
                  await geolocation.GetLocationAsync(
                     new GeolocationRequest(GeolocationAccuracy.Best));
               MyLocationLayer.UpdateMyLocation(
                  new Mapsui.UI.Forms.Position(currentLocation.Latitude, currentLocation.Longitude));
            });

            return runLocationTracker;
         });
      }

      private async Task Download() {

         if (connectivity.NetworkAccess == NetworkAccess.Internet) {
            IsLoading = true;
            await dataService.DownloadArea(Info);
            IsLoading = false;
         } else {
            var dialogParams = new DialogParameters();
            dialogParams.Add(
               DialogPageViewModel.ParameterKeys.Message,
               Strings.UnableToDownloadEntireArea);
            dialogParams.Add(DialogPageViewModel.ParameterKeys.Severity, DialogPageViewModel.Severity.Error);

            await DialogService.ShowDialogAsync("Unable to download", dialogParams);
         }
      }
   }
}
