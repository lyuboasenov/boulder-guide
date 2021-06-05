using BruTile;
using BruTile.MbTiles;
using BruTile.Predefined;
using BruTile.Web;
using ClimbingMap.Domain.Entities;
using ClimbingMap.Mobile.Forms.Services.Data;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Widgets.ScaleBar;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials.Interfaces;

namespace ClimbingMap.Mobile.Forms.Services.Maps {
   public class MapService : IMapService {
      private readonly IConnectivity connectivity;

      public MapService(IConnectivity connectivity) {
         this.connectivity = connectivity;
      }

      public Map GetMap(Area area, AreaInfo info) {
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

      public Map GetMap(Route route, AreaInfo info) {
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

      private Map GetMap(AreaInfo info) {
         var map = new Map {
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
               Fill = new Brush(new Color(150, 150, 30, 64)),
               Outline = new Pen {
                  Color = Color.Orange,
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
