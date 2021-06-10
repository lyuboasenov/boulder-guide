﻿using BruTile.MbTiles;
using BoulderGuide.Domain.Entities;
using BoulderGuide.Mobile.Forms.Services.Data;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Essentials.Interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

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
         IGeolocation geolocation) {
         this.connectivity = connectivity;
         this.permissions = permissions;
         this.preferences = preferences;
         this.geolocation = geolocation;
      }

      public async Task StartLocationPollingAsync() {
         lock (_lock) {
            _locationPollersCount++;
         }
         System.Threading.Interlocked.Increment(ref _locationPollersCount);
         if (await permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Granted) {
            Device.StartTimer(TimeSpan.FromSeconds(preferences.GPSPollIntervalInSeconds), () => {
               Task.Run(async () => {
                  if (_locationPollersCount > 0) {
                     var currentLocation =
                        await geolocation.GetLocationAsync(locationRequest).ConfigureAwait(false);

                     LocationUpdated?.Invoke(this, new LocationUpdatedEventArgs(currentLocation.Latitude, currentLocation.Longitude));
                  }
               });

               return _locationPollersCount > 0;
            });
         }
      }

      public Task StopLocationPollingAsync() {
         lock(_lock) {
            _locationPollersCount = Math.Max(0, _locationPollersCount - 1);
         }
         return Task.CompletedTask;
      }

      public Mapsui.Map GetMap(Area area, AreaInfo info) {
         var map = GetMap(info);

         // Add area outline
         var polygonLayer = CreateOutlineLayer(area);
         map.Layers.Add(polygonLayer);
         map.Home = n =>
            n.NavigateTo(
               polygonLayer.Envelope.Centroid,
               10);

         // Add area routes
         if ((info.Routes?.Length ?? 0) > 0) {
            map.Layers.Add(CreateRoutesLayer(info));
         }

         return map;
      }

      public Mapsui.Map GetMap(Route route, AreaInfo info) {
         var map = GetMap(info);

         var routeLayer = CreateRouteLayer(route);
         map.Layers.Add(routeLayer);
         map.Home = n =>
            n.NavigateTo(
               routeLayer.Envelope.Centroid, 1);

         return map;
      }

      private ILayer CreateRouteLayer(Route route) {

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

      private Mapsui.Map GetMap(AreaInfo info) {
         var map = new Mapsui.Map {
            CRS = "EPSG:3857",
            Transformation = new MinimalTransformation()
         };

         if (connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet) {
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
         } else if (File.Exists(info.MapLocalPath)) {
            var mbTilesTileSource =
               new MbTilesTileSource(
                  new SQLiteConnectionString(info.MapLocalPath, true),
                  type: MbTilesType.BaseLayer);
            map.Layers.Add(new TileLayer(mbTilesTileSource) { Name = "MbTiles" });
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

         // Fails
         result.Add(
            new Polygon(
               new LinearRing(
                  area.Location.Select(p => SphericalMercator.FromLonLat(p.Longitude, p.Latitude)))));

         return result;
      }
   }
}