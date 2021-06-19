using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Services.Data.Entities;
using BoulderGuide.Mobile.Forms.Services.Location;
using Prism.Navigation;
using System.Threading.Tasks;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MapPageViewModel : ViewModelBase {

      private readonly ILocationService locationService;

      public Mapsui.Map Map { get; set; }
      public Mapsui.UI.Objects.MyLocationLayer MyLocationLayer { get; set; }
      public MapPageViewModel(
         ILocationService locationService) {
         this.locationService = locationService;

         locationService.LocationUpdated += LocationService_LocationUpdated;
      }

      public override void Initialize(INavigationParameters parameters) {
         base.Initialize(parameters);

         if (parameters.TryGetValue(nameof(RouteInfo), out RouteInfo routeInfo)) {
            // route mode
            Map = locationService.GetMap(routeInfo);
            Title = $"{routeInfo.Name} ({new Grade(routeInfo.Difficulty)})";
         } else if (parameters.TryGetValue(nameof(AreaInfo), out AreaInfo areaInfo)) {
            // area mode
            Map = locationService.GetMap(areaInfo);
            Title = areaInfo.Name;
         } else {
            NavigationService.GoBackAsync();
         }
         Task.Run(async () => await InitializeAsync());
      }

      public override void Destroy() {
         base.Destroy();
         Task.Run(async () => await locationService.StopLocationPollingAsync());
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
