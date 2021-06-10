using BoulderGuide.Domain.Entities;
using BoulderGuide.Mobile.Forms.Services.Data;
using BoulderGuide.Mobile.Forms.Services.Location;
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

      private readonly IGeolocation geolocation;
      private readonly IPermissions permissions;
      private readonly ILocationService locationService;
      private readonly Services.Preferences.IPreferences preferences;

      public Mapsui.Map Map { get; set; }
      public Mapsui.UI.Objects.MyLocationLayer MyLocationLayer { get; set; }
      public MapPageViewModel(
         ILocationService locationService,
         IGeolocation geolocation,
         IPermissions permissions,
         Services.Preferences.IPreferences preferences) {
         this.locationService = locationService;
         this.geolocation = geolocation;
         this.permissions = permissions;
         this.preferences = preferences;

         locationService.LocationUpdated += LocationService_LocationUpdated;
      }

      public override void OnNavigatedTo(INavigationParameters parameters) {
         base.OnNavigatedTo(parameters);

         if (parameters.TryGetValue(nameof(AreaInfo), out AreaInfo areaInfo)) {
            if (parameters.TryGetValue(nameof(Route), out Route route)) {
               // route mode
               Map = locationService.GetMap(route, areaInfo);
               Title = $"{route.Name} ({route.Grade})";
            } else if (parameters.TryGetValue(nameof(Area), out Area area)) {
               // area mode
               Map = locationService.GetMap(area, areaInfo);
               Title = area.Name;
            } else {
               NavigationService.GoBackAsync();
            }
            Task.Run(async () => await InitializeAsync());
         } else {
            Task.Run(async () => await NavigationService.GoBackAsync());
         }
      }

      private void LocationService_LocationUpdated(object sender, LocationUpdatedEventArgs e) {
         MyLocationLayer?.UpdateMyLocation(
                        new Mapsui.UI.Forms.Position(e.Latitude, e.Longitude));
      }

      private Task InitializeAsync() {
         return locationService.StartLocationPollingAsync();
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
