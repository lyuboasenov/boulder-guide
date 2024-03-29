﻿using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Location;
using BruTile.MbTiles;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Widgets.ScaleBar;
using Prism.Navigation;
using SQLite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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

      private ILayer connectingLayer;
      private IFeature feature;
      private LineString line;
      private static readonly ConcurrentDictionary<string, int> imageCache = new ConcurrentDictionary<string, int>();

      public string Title { get; set; }
      public Mapsui.Map Map { get; set; }
      public double MapResolution { get; set; } = 2;
      public double MapMinResolution { get; set; } = 0.2;
      public double MapMaxResolution { get; set; } = 20000;
      public string Distance { get; set; }
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

            FollowMode = Views.FollowMode.TargetLocation;
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

         if (info.Area.Tracks?.Any() ?? false) {
            map.Layers.Add(CreateTracksLayer(info));
         }

         if (info.Area.POIs?.Any() ?? false) {
            map.Layers.Add(CreatePOIsLayer(info));
         }

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

      private ILayer CreatePOIsLayer(AreaInfo info) {

         var features = new Features();

         foreach (var poi in info.Area.POIs ?? Enumerable.Empty<PointOfInterest>()) {
            features.Add(CreateSvgFeature(poi));
         }

         return new Layer("POIs layer") {
            Style = null,
            DataSource = new MemoryProvider(features)
         };
      }

      private ILayer CreateTracksLayer(AreaInfo info) {
         var features = new List<IFeature>();
         foreach (var track in info.Area.Tracks ?? Enumerable.Empty<Track>()) {
            features.Add(
               new Feature() {
                  Geometry = new LineString(
                  track.Locations.Select(
                     l => SphericalMercator.FromLonLat(l.Longitude, l.Latitude)))
               });
         }


         return new Layer("Tracks layer") {
            DataSource = new MemoryProvider(features),
            Style = new VectorStyle {
               Fill = null,
               Outline = null,
               Line = { Color = new Mapsui.Styles.Color(105, 105, 105), Width = 2, PenStyle = PenStyle.DashDot }
            }
         };
      }

      private ILayer CreateRouteLayer(RouteInfo route) {
         return CreateRoutesLayer(new[] { GetRouteFeature(
               SphericalMercator.FromLonLat(route.Location.Longitude, route.Location.Latitude),
               $"{route.Name} ({new Grade(route.Difficulty)})") });
      }

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

         map.Widgets.Add(
            new ScaleBarWidget(map) {
               ScaleBarMode = ScaleBarMode.Both,
               MarginX = 10,
               MarginY = 60,
               UnitConverter = MetricBGUnitConverter.instance
            });

         connectingLayer = CreateConnectingLayer();
         map.Layers.Add(connectingLayer);

         return map;
      }

      private ILayer CreateConnectingLayer() {

         line = new LineString(new[] {
                  new Mapsui.Geometries.Point(0, 0),
                  new Mapsui.Geometries.Point(0, 0)
            });

         feature = new Feature {
            Geometry = line,
            Styles = new[] {
                  new VectorStyle {
                      Line = new Pen(Mapsui.Styles.Color.Gray) {
                         PenStyle = PenStyle.Dash,
                         PenStrokeCap = PenStrokeCap.Butt,
                         StrokeJoin = StrokeJoin.Miter
                      }
                  },
               }
         };

         return new Layer("Connecting layer") {
            DataSource = new MemoryProvider(new[] { feature }),
            Style = null
         };
      }

      private ILayer CreateRoutesLayer(AreaInfo info) {
         var features = new List<IFeature>();

         foreach (var gr in info.Routes?.GroupBy(r => r.Location, new LocationComparer()) ?? Enumerable.Empty<IGrouping<DTOs.Location, RouteInfo>>()) {
            var sb = new StringBuilder();
            foreach (var route in gr) {
               sb.AppendLine($"{route.Name} ({new Grade(route.Difficulty)})");
            }
            features.Add(GetRouteFeature(
               SphericalMercator.FromLonLat(gr.Key.Longitude, gr.Key.Latitude),
               sb.ToString().Trim()));
         }

         return CreateRoutesLayer(features);
      }

      private ILayer CreateRoutesLayer(IEnumerable<IFeature> features) {
         return new Layer("Routes layer") {
            Style = CreateSvgStyle("BoulderGuide.Mobile.Forms.Icons.place.svg"),
            DataSource = new MemoryProvider(features)
         };
      }

      private IFeature GetRouteFeature(IGeometry geometry, string text) {
         var feature = new Feature() {
            Geometry = geometry
         };

         feature.Styles.Add(new LabelStyle() {
            Text = text,
            HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Left,
            Offset = new Offset(15, 0),
            BackColor = new Mapsui.Styles.Brush(new Mapsui.Styles.Color(255, 255, 255, 128))
         });

         return feature;
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

      private IFeature CreateSvgFeature(PointOfInterest poi) {
         var point = SphericalMercator.FromLonLat(poi.Location.Longitude, poi.Location.Latitude);
         string id = "BoulderGuide.Mobile.Forms.Icons.parking.svg";
         if (poi.Type == "water") {
            id = "BoulderGuide.Mobile.Forms.Icons.local_drink.svg";
         }
         return CreateSvgFeature(point, id, poi.Name);
      }

      private IFeature CreateSvgFeature(Mapsui.Geometries.Point point, string imageId, string label) {
         var feature = new Feature { Geometry = point, ["Label"] = label };
         feature.Styles.Add(CreateSvgStyle(imageId));

         return feature;
      }

      private SymbolStyle CreateSvgStyle(string embeddedResourcePath) {
         var bitmapId = GetBitmapIdForEmbeddedResource(embeddedResourcePath);
         var style = new SymbolStyle { BitmapId = bitmapId };

         return style;
      }

      private int GetBitmapIdForEmbeddedResource(string imagePath) {
         if (!imageCache.TryGetValue(imagePath, out var id)) {
            try {
               var assembly = typeof(MapPageViewModel).GetTypeInfo().Assembly;
               var image = assembly.GetManifestResourceStream(imagePath);
               id = BitmapRegistry.Instance.Register(image);
               imageCache[imagePath] = id;
            } catch {
               // Silence
            }
         }

         return id;
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

         if (TargetLocation?.Any() == true && null != MyLocation) {
            // update connecting line
            var targetBox = new BoundingBox(TargetLocation.Select(p =>
            SphericalMercator.FromLonLat(p.Longitude, p.Latitude)));
            var myLocationPoint = SphericalMercator.FromLonLat(MyLocation.Longitude, MyLocation.Latitude);

            Distance = MetricBGUnitConverter.instance.GetScaleText(
               (int) targetBox.Centroid.Distance(myLocationPoint));

            line.Vertices.Clear();
            line.Vertices.Add(targetBox.Centroid);
            line.Vertices.Add(myLocationPoint);

            feature.RenderedGeometry.Clear();

            connectingLayer.DataHasChanged();
         }
      }
   }

   internal class MetricBGUnitConverter : IUnitConverter {
      private static readonly int _oneKilometer = 1000;

      internal static IUnitConverter instance = new MetricBGUnitConverter();
      public double MeterRatio => 1;

      public IEnumerable<int> ScaleBarValues { get; } = new[] {
            10000000, 5000000, 2000000, 1000000, 500000, 200000, 100000, 50000,
            20000, 10000, 5000, 2000, 1000, 500, 200, 100, 50, 20, 10, 5, 2, 1

        };

      public string GetScaleText(int mapScaleValue) {
         if (mapScaleValue < _oneKilometer) {
            return mapScaleValue + " м";
         }
         return (mapScaleValue / _oneKilometer) + " км";
      }
   }

   public class LocationComparer : EqualityComparer<DTOs.Location> {
      private const double DELTA = 0.0001;
      public override bool Equals(DTOs.Location x, DTOs.Location y) {
         if (x == null && y == null)
            return true;
         else if (x == null || y == null)
            return false;

         return Math.Abs(x.Latitude - y.Latitude) < DELTA && Math.Abs(x.Longitude - y.Longitude) < DELTA;
      }

      public override int GetHashCode(DTOs.Location obj) {
         return 1;
      }
   }
}
