﻿using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Location;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MapPageViewModel : ViewModelBase {

      private readonly ILocationService locationService;

      public string Title { get; set; }
      public Mapsui.Map Map { get; set; }
      public double MapResolution { get; set; }

      public void OnMapResolutionChanged() {
         RunOnMainThreadAsync(() => {
            (ZoomInCommand as Command)?.ChangeCanExecute();
            (ZoomOutCommand as Command)?.ChangeCanExecute();
         });
      }
      public double MapRotation { get; set; }
      public void OnMapRotationChanged() {
         RunOnMainThreadAsync(() => {
            (NorthCommand as Command)?.ChangeCanExecute();
         });
      }
      public Mapsui.UI.Objects.MyLocationLayer MyLocationLayer { get; set; }
      public bool FollowMyLocation { get; set; }
      public ICommand GoToMyLocationCommand { get; }
      public ICommand ZoomInCommand { get; }
      public ICommand ZoomOutCommand { get; }
      public ICommand NorthCommand { get; }
      public MapPageViewModel(
         ILocationService locationService) {
         this.locationService = locationService;
         GoToMyLocationCommand = new Command(GoToMyLocation);
         ZoomInCommand = new Command(ZoomIn, CanZoomIn);
         ZoomOutCommand = new Command(ZoomOut, CanZoomOut);
         NorthCommand = new Command(North, CanNorth);

         locationService.LocationUpdated += LocationService_LocationUpdated;
      }

      public override bool CanNavigate(INavigationParameters parameters) {
         return base.CanNavigate(parameters) &&
            (parameters.TryGetValue(nameof(RouteInfo), out RouteInfo _) ||
            parameters.TryGetValue(nameof(AreaInfo), out AreaInfo __));
      }

      public override async Task InitializeAsync(INavigationParameters parameters) {
         try {
            await base.InitializeAsync(parameters);

            if (parameters.TryGetValue(nameof(RouteInfo), out RouteInfo routeInfo)) {
               // route mode
               Map = locationService.GetMap(routeInfo);
               Title = $"{routeInfo.Name} ({new Grade(routeInfo.Difficulty)})";
            } else if (parameters.TryGetValue(nameof(AreaInfo), out AreaInfo areaInfo)) {
               // area mode
               Map = locationService.GetMap(areaInfo);
               Title = areaInfo.Name;
            }

            await locationService.StartLocationPollingAsync();
         } catch (Exception ex) {
            HandleOperationException(ex, Strings.UnableToInitializeMap);
         }
      }

      public override void Destroy() {
         base.Destroy();
         RunAsync(locationService.StopLocationPollingAsync);
      }


      private bool CanNorth() {
         return MapRotation != 0;
      }

      private void North() {
         MapRotation = 0;
      }

      private void LocationService_LocationUpdated(object sender, LocationUpdatedEventArgs e) {
         MyLocationLayer?.UpdateMyLocation(
            new Mapsui.UI.Forms.Position(e.Latitude, e.Longitude));
      }

      private void GoToMyLocation() {
         FollowMyLocation = true;
         FollowMyLocation = false;
      }

      private bool CanZoomOut() {
         return MapResolution < 20000;
      }

      private void ZoomOut() {
         MapResolution *= 1.6;
      }

      private bool CanZoomIn() {
         return MapResolution > 0.2;
      }

      private void ZoomIn() {
         MapResolution /= 1.6;
      }

      public static NavigationParameters InitializeParameters(RouteInfo routeInfo) {
         return InitializeParameters(nameof(RouteInfo), routeInfo);
      }

      public static NavigationParameters InitializeParameters(AreaInfo areaInfo) {
         return InitializeParameters(nameof(AreaInfo), areaInfo);
      }
   }
}
