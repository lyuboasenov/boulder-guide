﻿using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using BoulderGuide.Mobile.Forms.Services.Location;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MapPageViewModel : ViewModelBase {

      private readonly ILocationService locationService;

      public Mapsui.Map Map { get; set; }
      public Mapsui.UI.Objects.MyLocationLayer MyLocationLayer { get; set; }
      public ICommand GoBackCommand { get; }
      public MapPageViewModel(
         ILocationService locationService) {
         this.locationService = locationService;
         GoBackCommand = new AsyncCommand(GoBackAsync);

         locationService.LocationUpdated += LocationService_LocationUpdated;
      }

      public override async Task InitializeAsync(INavigationParameters parameters) {
         await base.InitializeAsync(parameters);

         if (parameters.TryGetValue(nameof(RouteInfo), out RouteInfo routeInfo)) {
            // route mode
            Map = locationService.GetMap(routeInfo);
            Title = $"{routeInfo.Name} ({new Grade(routeInfo.Difficulty)})";
         } else if (parameters.TryGetValue(nameof(AreaInfo), out AreaInfo areaInfo)) {
            // area mode
            Map = locationService.GetMap(areaInfo);
            Title = areaInfo.Name;
         } else {
            await GoBackAsync();
         }
         await InitializeAsync();
      }

      public override void Destroy() {
         base.Destroy();
         RunAsync(locationService.StopLocationPollingAsync);
      }

      private void LocationService_LocationUpdated(object sender, LocationUpdatedEventArgs e) {
         MyLocationLayer?.UpdateMyLocation(
            new Mapsui.UI.Forms.Position(e.Latitude, e.Longitude));
      }

      private Task InitializeAsync() {
         return locationService.StartLocationPollingAsync();
      }

      public static NavigationParameters InitializeParameters(RouteInfo routeInfo) {
         return InitializeParameters(nameof(RouteInfo), routeInfo);
      }

      public static NavigationParameters InitializeParameters(AreaInfo areaInfo) {
         return InitializeParameters(nameof(AreaInfo), areaInfo);
      }
   }
}
