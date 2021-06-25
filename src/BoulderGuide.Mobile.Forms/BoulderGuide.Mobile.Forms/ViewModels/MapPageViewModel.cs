﻿using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Location;
using BruTile.MbTiles;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using Prism.Navigation;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace BoulderGuide.Mobile.Forms.ViewModels {
   public class MapPageViewModel : ViewModelBase, ILocationObserver {

      private readonly ILocationService locationService;
      private readonly Services.Preferences.IPreferences preferences;
      private readonly IConnectivity connectivity;

      public string Title { get; set; }
      public Mapsui.Map Map { get; set; }
      public double MapResolution { get; set; } = 2;
      public double MapMinResolution { get; set; } = 0.2;
      public double MapMaxResolution { get; set; } = 20000;

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
      public Mapsui.UI.Forms.Position MyLocation { get; set; }
      public double MyDirection { get; set; }
      public IEnumerable<Mapsui.UI.Forms.Position> TargetLocation { get; set; }

      public Views.FollowMode FollowMode { get; set; }

      public void OnFollowModeChanged() {
         (GoToMyLocationCommand as Command)?.ChangeCanExecute();
         (GoToTargetCommand as Command)?.ChangeCanExecute();
      }

      private readonly IDisposable locationPollHandle;

      public ICommand GoToMyLocationCommand { get; }
      public ICommand GoToTargetCommand { get; }
      public ICommand ZoomInCommand { get; }
      public ICommand ZoomOutCommand { get; }
      public ICommand NorthCommand { get; }

      public MapPageViewModel(
         ILocationService locationService,
         Services.Preferences.IPreferences preferences,
         IConnectivity connectivity) {
         this.locationService = locationService;
         this.preferences = preferences;
         this.connectivity = connectivity;

         GoToMyLocationCommand = new Command(GoToMyLocation, () => FollowMode != Views.FollowMode.MyLocation);
         GoToTargetCommand = new Command(GoToTarget, () => FollowMode != Views.FollowMode.TargetLocation);
         ZoomInCommand = new Command(ZoomIn, CanZoomIn);
         ZoomOutCommand = new Command(ZoomOut, CanZoomOut);
         NorthCommand = new Command(North, CanNorth);

         locationPollHandle = locationService.Subscribe(this);
         MyLocation
            = new Mapsui.UI.Forms.Position(preferences.LastKnownLatitude, preferences.LastKnownLongitude);
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
               Map = GetMap(routeInfo);
               Title = $"{routeInfo.Name} ({new Grade(routeInfo.Difficulty)})";
               TargetLocation = new[] { new Mapsui.UI.Forms.Position(routeInfo.Location.Latitude, routeInfo.Location.Longitude) };
            } else if (parameters.TryGetValue(nameof(AreaInfo), out AreaInfo areaInfo)) {
               // area mode
               Map = GetMap(areaInfo);
               Title = areaInfo.Name;
               TargetLocation = areaInfo.Area.Location.Select(l =>
                  new Mapsui.UI.Forms.Position(l.Latitude, l.Longitude));
            }

            GoToTargetCommand.Execute(null);

            //if (FollowMyLocation) {
            //   MapResolution = 2;
            //   Map.Home = n => n.NavigateTo(
            //      SphericalMercator.FromLonLat(MyLocation.Longitude, MyLocation.Latitude),
            //      MapResolution);
            //} else {
            //   Map.Home = n => n.NavigateTo(
            //      new BoundingBox(TargetLocation.Select(l =>
            //      SphericalMercator.FromLonLat(l.Longitude, l.Latitude))), ScaleMethod.Fit);
            //}
         } catch (Exception ex) {
            await HandleOperationExceptionAsync(ex, Strings.UnableToInitializeMap);
         }
      }

      public override void Destroy() {
         base.Destroy();
         locationPollHandle?.Dispose();
      }


      private bool CanNorth() {
         return MapRotation != 0;
      }

      private void North() {
         MapRotation = 0;
      }

      private void GoToMyLocation() {
         FollowMode = Views.FollowMode.MyLocation;
      }

      private void GoToTarget() {
         FollowMode = Views.FollowMode.TargetLocation;
      }

      private bool CanZoomOut() {
         return MapResolution < MapMaxResolution;
      }

      private void ZoomOut() {
         MapResolution = Math.Min(MapResolution * 1.6, MapMaxResolution);
      }

      private bool CanZoomIn() {
         return MapResolution > MapMinResolution;
      }

      private void ZoomIn() {
         MapResolution = Math.Max(MapResolution / 1.6, MapMinResolution);
      }


      private Mapsui.Map GetMap(AreaInfo info) {
         var map = GetBaseMap(info);

         // Add area outline
         var polygonLayer = CreateOutlineLayer(info.Area);
         map.Layers.Add(polygonLayer);

         map.Home = n =>
            n.NavigateTo(
               polygonLayer.Envelope);

         // Add area routes
         if (info.Routes?.Any() ?? false) {
            map.Layers.Add(CreateRoutesLayer(info));
         }

         return map;
      }

      private Mapsui.Map GetMap(RouteInfo info) {
         var map = GetBaseMap(info.Parent);

         var routeLayer = CreateRouteLayer(info);
         map.Layers.Add(routeLayer);

         map.Home = n =>
            n.NavigateTo(
               routeLayer.Envelope.Centroid, 1);

         return map;
      }

      private ILayer CreateRouteLayer(RouteInfo route) {

         var feature = new Feature() {
            Geometry = SphericalMercator.FromLonLat(route.Location.Longitude, route.Location.Latitude)
         };
         feature.Styles.Add(new LabelStyle() {
            Text = $"{route.Name} ({new Grade(route.Difficulty)})",
            HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Left
         });

         return new Layer("Routes layer") {
            DataSource = new MemoryProvider(new[] { feature })
         };
      }

      private static readonly BruTile.Attribution OpenStreetMapAttribution = new BruTile.Attribution(
            "© OpenStreetMap contributors", "https://www.openstreetmap.org/copyright");

      private Mapsui.Map GetBaseMap(AreaInfo info) {
         var map = new Mapsui.Map {
            CRS = "EPSG:3857",
            Transformation = new MinimalTransformation()
         };

         if (connectivity.NetworkAccess == NetworkAccess.Internet) {
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
         } else {

            while (info != null && (!info.Map?.ExistsLocally ?? true)) {
               info = info.Parent;
            }

            if (info?.Map?.ExistsLocally ?? false) {
               var mbTilesTileSource =
               new MbTilesTileSource(
                  new SQLiteConnectionString(info.Map.GetMapLocalFilePath(), true),
                  type: MbTilesType.BaseLayer);
               map.Layers.Add(new TileLayer(mbTilesTileSource) { Name = "MbTiles" });
            }
         }

         return map;
      }

      private ILayer CreateRoutesLayer(AreaInfo info) {
         var features = new List<IFeature>();

         foreach (var route in info.Routes ?? Enumerable.Empty<RouteInfo>()) {
            var feature = new Feature() {
               Geometry = SphericalMercator.FromLonLat(route.Location.Longitude, route.Location.Latitude)
            };
            feature.Styles.Add(new LabelStyle() {
               Text = $"{route.Name} ({new Grade(route.Difficulty)})",
               HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Left
            });
            features.Add(feature);
         }

         return new Layer("Routes layer") {
            DataSource = new MemoryProvider(features)
         };
      }

      private ILayer CreateOutlineLayer(Area area) {
         return new Layer("Outline layer") {
            DataSource = new MemoryProvider(CreatePolygon(area)),
            Style = new VectorStyle {
               Fill = new Mapsui.Styles.Brush(new Mapsui.Styles.Color(150, 150, 30, 64)),
               Outline = new Pen {
                  Color = Mapsui.Styles.Color.Orange,
                  Width = 2,
                  PenStyle = PenStyle.Solid,
                  PenStrokeCap = PenStrokeCap.Round
               }
            }
         };
      }

      private IEnumerable<IGeometry> CreatePolygon(Area area) {
         var result = new List<Polygon>();

         if (area?.Location?.Any() ?? false) {
            // Fails
            result.Add(
               new Polygon(
                  new LinearRing(
                     area?.Location?.Select(p => SphericalMercator.FromLonLat(p.Longitude, p.Latitude)))));
         }

         return result;
      }

      public static NavigationParameters InitializeParameters(RouteInfo routeInfo) {
         return InitializeParameters(nameof(RouteInfo), routeInfo);
      }

      public static NavigationParameters InitializeParameters(AreaInfo areaInfo) {
         return InitializeParameters(nameof(AreaInfo), areaInfo);
      }

      public void OnLocationChanged(double latitude, double longitude, double direction) {
         MyLocation =
            new Mapsui.UI.Forms.Position(latitude, longitude);

         MyDirection = direction;
      }
   }
}
