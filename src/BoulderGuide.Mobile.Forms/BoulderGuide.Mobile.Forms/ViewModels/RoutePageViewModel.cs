using BoulderGuide.Domain.Entities;
using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.Maps;
using BoulderGuide.Mobile.Forms.Views;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class RoutePageViewModel : ViewModelBase {

      private readonly IDataService dataService;
      private readonly IConnectivity connectivity;
      private readonly IMapService mapService;
      private bool runLocationTracker = false;

      public Route Route { get; set; }
      public RouteInfo Info { get; set; }
      public AreaInfo AreaInfo { get; set; }
      public Mapsui.Map Map { get; set; }
      public Mapsui.UI.Objects.MyLocationLayer MyLocationLayer { get; set; }

      public RoutePageViewModel(
         INavigationService navigationService,
         IDataService dataService,
         IConnectivity connectivity,
         IDialogService dialogService,
         IMapService mapService,
         IGeolocation geolocation,
         IPermissions permissions) :
         base(navigationService, dialogService) {
         this.dataService = dataService;
         this.connectivity = connectivity;
         this.mapService = mapService;
         this.geolocation = geolocation;
         this.permissions = permissions;
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         if (parameters.TryGetValue(ParameterKeys.RouteInfo, out RouteInfo info) &&
            parameters.TryGetValue(ParameterKeys.AreaInfo, out AreaInfo areaInfo)) {
            Task.Run(async () => await InitializeAsync(info, areaInfo));
         } else {
            Task.Run(async () => await NavigationService.GoBackAsync());
         }
         runLocationTracker = true;
      }

      public override void OnNavigatedFrom(INavigationParameters parameters) {
         base.OnNavigatedFrom(parameters);
         runLocationTracker = false;
      }

      private async Task InitializeAsync(RouteInfo info, AreaInfo areaInfo) {
         Info = info;
         AreaInfo = areaInfo;

         try {
            if (connectivity.NetworkAccess == NetworkAccess.Internet) {
               Route = await dataService.GetRoute(Info);
            } else if (Info.IsOffline) {
               Route = await dataService.GetOfflineRoute(Info);
            } else {
               var dialogParams = new DialogParameters();
               dialogParams.Add(
                  DialogPageViewModel.ParameterKeys.Message,
                  Strings.UnableToDownloadRoute);
               dialogParams.Add(DialogPageViewModel.ParameterKeys.Severity, DialogPageViewModel.Severity.Error);

               await DialogService.ShowDialogAsync(nameof(DialogPage), dialogParams);
            }

            Map = mapService.GetMap(Route, AreaInfo);
            if (await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted) {
               RunLocationTracker();
            }
         } catch (Exception ex) {
            await HandleExceptionAsync(ex);
         }
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

      public const string View = "RoutePage";
      private readonly IGeolocation geolocation;
      private readonly IPermissions permissions;

      public static class ParameterKeys {
         public const string RouteInfo = "RouteInfo";
         public const string AreaInfo = "AreaInfo";
      }
   }
}
