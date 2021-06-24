using BruTile.MbTiles;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using BoulderGuide.DTOs;
using BoulderGuide.Mobile.Forms.Domain;
using BoulderGuide.Mobile.Forms.Services.Errors;

namespace BoulderGuide.Mobile.Forms.Services.Location {
   public class LocationService : ILocationService {
      private readonly IConnectivity connectivity;
      private int _locationPollersCount;
      private readonly object _lock = new object();
      private readonly IPermissions permissions;
      private readonly Preferences.IPreferences preferences;
      private readonly IGeolocation geolocation;
      private static readonly GeolocationRequest locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);


      public event EventHandler<LocationUpdatedEventArgs> LocationUpdated;

      public LocationService(
         IConnectivity connectivity,
         IPermissions permissions,
         Preferences.IPreferences preferences,
         IGeolocation geolocation,
         IErrorService errorService) {
         this.connectivity = connectivity;
         this.permissions = permissions;
         this.preferences = preferences;
         this.geolocation = geolocation;
         this.errorService = errorService;
      }

      public async Task StartLocationPollingAsync() {
         lock (_lock) {
            _locationPollersCount++;
         }

         if (await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted) {
            Device.StartTimer(TimeSpan.FromSeconds(preferences.GPSPollIntervalInSeconds), () => {
               Task.Run(async () => {
                  try {
                     if (_locationPollersCount > 0) {

                        if (lastKnownLocation is null) {
                           lastKnownLocation = await geolocation.GetLastKnownLocationAsync();
                        }

                        if (null != lastKnownLocation) {
                           LocationUpdated?.Invoke(this, new LocationUpdatedEventArgs(lastKnownLocation.Latitude, lastKnownLocation.Longitude));
                        }

                        lastKnownLocation =
                           await geolocation.GetLocationAsync(locationRequest).ConfigureAwait(false);

                        if (null != lastKnownLocation) {
                           LocationUpdated?.Invoke(this, new LocationUpdatedEventArgs(lastKnownLocation.Latitude, lastKnownLocation.Longitude));
                        }
                     }
                  } catch (Exception ex) {
                     await errorService.HandleErrorAsync(ex, true);
                  }
               });

               return _locationPollersCount > 0;
            });
         }
      }

      private Xamarin.Essentials.Location lastKnownLocation;
      private readonly IErrorService errorService;

      public Task StopLocationPollingAsync() {
         lock(_lock) {
            _locationPollersCount = Math.Max(0, _locationPollersCount - 1);
         }
         return Task.CompletedTask;
      }

      public Mapsui.Map GetMap(AreaInfo info) {
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

      public Mapsui.Map GetMap(RouteInfo info) {
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
   }
}
