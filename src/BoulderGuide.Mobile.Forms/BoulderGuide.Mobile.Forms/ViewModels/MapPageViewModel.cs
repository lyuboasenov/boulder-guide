using BoulderGuide.Domain.Entities;
using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.Maps;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MapPageViewModel : ViewModelBase {

      private static readonly GeolocationRequest locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
      private readonly IGeolocation geolocation;
      private readonly IPermissions permissions;
      private readonly IMapService mapService;
      private readonly Services.Preferences.IPreferences preferences;
      private bool runLocationTracker;

      public Mapsui.Map Map { get; set; }
      public Mapsui.UI.Objects.MyLocationLayer MyLocationLayer { get; set; }
      public MapPageViewModel(
         INavigationService navigationService,
         IDialogService dialogService,
         IMapService mapService,
         IGeolocation geolocation,
         IPermissions permissions,
         Services.Preferences.IPreferences preferences) :
         base(navigationService, dialogService) {
         this.mapService = mapService;
         this.geolocation = geolocation;
         this.permissions = permissions;
         this.preferences = preferences;
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         if (parameters.TryGetValue(nameof(AreaInfo), out AreaInfo areaInfo)) {
            if (parameters.TryGetValue(nameof(Route), out Route route)) {
               // route mode
               Map = mapService.GetMap(route, areaInfo);
               Title = $"{route.Name} ({route.Grade})";
            } else if (parameters.TryGetValue(nameof(Area), out Area area)) {
               // area mode
               Map = mapService.GetMap(area, areaInfo);
               Title = area.Name;
            } else {
               NavigationService.GoBackAsync();
            }
            Task.Run(async () => await InitializeAsync());
         } else {
            Task.Run(async () => await NavigationService.GoBackAsync());
         }
         runLocationTracker = true;
      }

      private async Task InitializeAsync() {
         if (await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted) {
            Device.StartTimer(TimeSpan.FromSeconds(preferences.GPSPollIntervalInSeconds), () => {
               Task.Run(async () => {
                  if (runLocationTracker) {
                     var currentLocation =
                        await geolocation.GetLocationAsync(locationRequest).ConfigureAwait(false);
                     MyLocationLayer?.UpdateMyLocation(
                        new Mapsui.UI.Forms.Position(currentLocation.Latitude, currentLocation.Longitude));
                  }
               });

               return runLocationTracker;
            });
         }
      }

      public override void OnNavigatedFrom(INavigationParameters parameters) {
         base.OnNavigatedFrom(parameters);
         runLocationTracker = false;
      }

      public static NavigationParameters InitializeParameters(Route route, AreaInfo areaInfo) {
         return InitializeParameters(
            new KeyValuePair<string, object>(nameof(Route), route),
            new KeyValuePair<string, object>(nameof(AreaInfo), areaInfo));
      }

      public static NavigationParameters InitializeParameters(Area area, AreaInfo areaInfo) {
         return InitializeParameters(
            new KeyValuePair<string, object>(nameof(Area), area),
            new KeyValuePair<string, object>(nameof(AreaInfo), areaInfo));
      }
   }
}
