using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Location;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MapPageViewModel : ViewModelBase {

      private readonly ILocationService locationService;

      public Mapsui.Map Map { get; set; }
      public double MapResolution { get; set; }
      public double MapRotation { get; set; }
      public Mapsui.UI.Objects.MyLocationLayer MyLocationLayer { get; set; }
      public bool FollowMyLocation { get; set; }
      public ICommand GoBackCommand { get; }
      public ICommand GoToMyLocationCommand { get; }
      public ICommand ZoomInCommand { get; }
      public ICommand ZoomOutCommand { get; }
      public ICommand NorthCommand { get; }
      public MapPageViewModel(
         ILocationService locationService) {
         this.locationService = locationService;
         GoBackCommand = new AsyncCommand(GoBackAsync);
         GoToMyLocationCommand = new AsyncCommand(GoToMyLocation);
         ZoomInCommand = new AsyncCommand(ZoomIn, CanZoomIn);
         ZoomOutCommand = new AsyncCommand(ZoomOut, CanZoomOut);
         NorthCommand = new AsyncCommand(North, CanNorth);

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

      public void OnMapResolutionChanged() {
         RunOnMainThreadAsync(() => {
            (ZoomInCommand as AsyncCommand)?.RaiseCanExecuteChanged();
            (ZoomOutCommand as AsyncCommand)?.RaiseCanExecuteChanged();
         });
      }

      public void OnMapRotationChanged() {
         RunOnMainThreadAsync(() => {
            (NorthCommand as AsyncCommand)?.RaiseCanExecuteChanged();
         });
      }


      private bool CanNorth(object arg) {
         return MapRotation != 0;
      }

      private Task North() {
         MapRotation = 0;

         return Task.CompletedTask;
      }

      private void LocationService_LocationUpdated(object sender, LocationUpdatedEventArgs e) {
         MyLocationLayer?.UpdateMyLocation(
            new Mapsui.UI.Forms.Position(e.Latitude, e.Longitude));
      }

      private Task InitializeAsync() {
         return locationService.StartLocationPollingAsync();
      }
      private Task GoToMyLocation() {
         FollowMyLocation = true;

         return Task.CompletedTask;
      }

      private bool CanZoomOut(object arg) {
         return MapResolution < 20000;
      }

      private Task ZoomOut() {
         MapResolution *= 1.6;
         return Task.CompletedTask;
      }

      private bool CanZoomIn(object arg) {
         return MapResolution > 0.2;
      }

      private Task ZoomIn() {
         MapResolution /= 1.6;
         return Task.CompletedTask;
      }

      public static NavigationParameters InitializeParameters(RouteInfo routeInfo) {
         return InitializeParameters(nameof(RouteInfo), routeInfo);
      }

      public static NavigationParameters InitializeParameters(AreaInfo areaInfo) {
         return InitializeParameters(nameof(AreaInfo), areaInfo);
      }
   }
}
