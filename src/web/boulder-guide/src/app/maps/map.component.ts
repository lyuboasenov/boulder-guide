import { NgZone, ChangeDetectorRef } from '@angular/core';
import { View, Feature, Map, Geolocation } from 'ol';
import { Coordinate } from 'ol/coordinate';
import { ScaleLine, defaults as DefaultControls } from 'ol/control';
import proj4 from 'proj4';
import VectorLayer from 'ol/layer/Vector';
import { register } from 'ol/proj/proj4';
import { get as GetProjection, transform } from 'ol/proj';
import { Extent } from 'ol/extent';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import { Location } from '../domain/Location';
import BaseLayer from 'ol/layer/Base';
import VectorSource from 'ol/source/Vector';
import { Text, Style, Icon } from 'ol/style';
import Stroke from 'ol/style/Stroke';
import Fill from 'ol/style/Fill';
import Point from 'ol/geom/Point';
import { RouteInfo } from '../domain/RouteInfo';
import LineString from 'ol/geom/LineString';
import { PointOfInterest } from '../domain/PointOfInterest';
import { Track } from '../domain/Track';
import Polygon from 'ol/geom/Polygon';
import CircleStyle from 'ol/style/Circle';
import { toSize } from 'ol/size';

export abstract class MapComponent {

   map!: Map;
   extent!: Extent;
   geolocation!: Geolocation;

   marker!: HTMLImageElement;

   positionStyle!: Style;
   markerDefaultIcon!: Icon;
   markerHeadingIcon!: Icon;

   static readonly markerHeadingIcon: string = 'assets/img/geolocation_marker_heading.png';
   static readonly markerDefaultIcon: string = 'assets/img/geolocation_marker.png';

   private static readonly projectionId: string = "EPSG:3857";

   constructor(private zone: NgZone, private cd: ChangeDetectorRef) { }

   public initMap(): void {
      if (!this.marker) {
         this.marker = document.getElementById("geolocation_marker") as HTMLImageElement;
      }

      if (!this.map) {
         this.zone.runOutsideAngular(() => this.initMapInternal())
      }
   }

   protected abstract getAdditionalLayers(): BaseLayer[];
   protected abstract getMapBorder(): Location[];

   private initMapInternal(): void {
      proj4.defs(MapComponent.projectionId, "+proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +wktext  +no_defs");
      register(proj4);

      let view = this.getView(this.getMapBorder());

      this.geolocation = new Geolocation({
         // enableHighAccuracy must be set to true to have the heading value.
         trackingOptions: {
            maximumAge: 0,
            enableHighAccuracy: true,
            timeout: 10000,
         },
         projection: view.getProjection(),
      });
      this.geolocation.on('error', function (error) {
         console.error(error);
      });

      this.map = new Map({
         layers: this.getLayers(),
         target: 'map',
         view: view,
         controls: DefaultControls().extend([
            new ScaleLine({}),
         ]),
      });

      this.geolocation.setTracking(true);

      this.map.getView().fit(this.extent);
   }

   private getLayers(): BaseLayer[] {
      let layers = [
         MapComponent.getOSMLayer(),
         this.getPositionLayer()
      ];

      for (let layer of this.getAdditionalLayers()) {
         layers.push(layer);
      }

      return layers;
   }

   protected getPositionLayer(): BaseLayer {
      const accuracyFeature = new Feature();
      const _this = this;

      const positionFeature = new Feature();
      this.positionStyle = new Style({
         image: new Icon({
            img: this.marker,
            imgSize: [22, 22]
         }),
      });
      positionFeature.setStyle(this.positionStyle);

      this.geolocation.on('change:accuracyGeometry', function () {
         accuracyFeature.setGeometry(_this.geolocation.getAccuracyGeometry());
      });

      this.geolocation.on('change:position', function () {
         const position  = _this.geolocation.getPosition();
         if (position) {
            positionFeature.setGeometry(new Point(position));
         }
      });

      this.geolocation.on('change:heading', function () {
         _this.updateOrientation();
      });

      return new VectorLayer({
         source: new VectorSource({
            features: [accuracyFeature, positionFeature],
         }),
      });
   }

   private updateOrientation(): void {
      const heading = this.geolocation.getHeading();
      const isHeading = this.marker.src === MapComponent.markerHeadingIcon;

      if (heading) {
         if (!isHeading) {
            this.marker.src = MapComponent.markerHeadingIcon;
            this.positionStyle.setImage(new Icon({
               img: this.marker,
               imgSize: [22, 37]
            }));
         }

         this.positionStyle.getImage().setRotation(heading);
      } else {
         if (isHeading) {
            this.marker.src = MapComponent.markerDefaultIcon;
            this.positionStyle.setImage(new Icon({
               img: this.marker,
               imgSize: [22, 22]
            }));
         }
      }
   }

   protected getView(locations: Location[]): View {
      var projection = GetProjection(MapComponent.projectionId);

      var minLat: number = 1000;
      var maxLat: number = 0;
      var minLon: number = 1000;
      var maxLon: number = 0;

      for (var loc of locations) {
         minLat = Math.min(minLat, loc.Latitude);
         maxLat = Math.max(maxLat, loc.Latitude);
         minLon = Math.min(minLon, loc.Longitude);
         maxLon = Math.max(maxLon, loc.Longitude);
      }

      var minCoo = MapComponent.fromLonLat(minLon, minLat);
      var maxCoo = MapComponent.fromLonLat(maxLon, maxLat);
      this.extent = [minCoo[0], minCoo[1], maxCoo[0], maxCoo[1]];

      var centerLon = (minLon + maxLon) / 2;
      var centerLat = (minLat + maxLat) / 2;

      var center = MapComponent.fromLonLat(centerLon, centerLat);

      return new View({
         center: center,
         zoom: 6,
         projection: projection,
      });
   }

   private static getOSMLayer(): BaseLayer {
      return new TileLayer({
         source: new OSM({})
      });
   }

   protected static getRoutesLayer(routes: RouteInfo[] | null): BaseLayer {

      var features: Feature[] = [];

      if (routes != null) {
         var groupedRoutes = MapComponent.groupRoutesByLocation(routes);
         for (var r of groupedRoutes) {
            var f = MapComponent.featureFromLocation(r.Location);
            f.setStyle(new Style({
               text: new Text({
                  text: r.Names,
                  offsetX: 10,
                  offsetY: -15,
                  rotation: 0,
                  textAlign: 'left',
                  fill: new Fill({
                     color: 'black',
                  })
               })
            }));
            features.push(f);
            features.push(MapComponent.featureFromLocation(r.Location));
         }
      }

      return new VectorLayer({
         source: new VectorSource({
            features: features,
         }),
         style: [
            new Style({
               text: new Text({
                  text: '\ue55f',
                  font: 'normal 24px "Material Icons"',
                  textBaseline: 'bottom',
                  fill: new Fill({
                     color: 'black',
                  })
               })
            })
         ]
      });
   }

   private static groupRoutesByLocation(routes: RouteInfo[]): { Location: Location, Routes: RouteInfo[], Names: string }[] {
      let result: { Location: Location, Routes: RouteInfo[], Names: string }[] = [];

      let delta: number = 0.0001;
      for (let route of routes) {
         let found = false;
         for (let item of result) {
            if (Location.equals(item.Location, route.location, delta)) {
               item.Routes.push(route);
               item.Names += '\n' + route.name;
               found = true;

               // recalculate center location
               let longitude: number = 0;
               let latitude: number = 0;
               for (let itemRoute of item.Routes) {
                  longitude += itemRoute.location.Longitude;
                  latitude += itemRoute.location.Latitude;
               }

               item.Location.Longitude = longitude / item.Routes.length;
               item.Location.Latitude = latitude / item.Routes.length;
            }
         }

         if (!found) {
            result.push({ Location: route.location, Routes: [ route ], Names: route.name });
         }
      }

      return result;
   }

   protected static getTrackLayer(tracks: Track[] | null): BaseLayer {
      var features: Feature[] = [];

      const parkingStyle = new Style({
         text: new Text({
            text: '\ue54f',
            font: 'normal 24px "Material Icons"',
            textBaseline: 'bottom',
            fill: new Fill({
               color: 'black',
            })
         })
      });
      const waterStyle = new Style({
         text: new Text({
            text: '\ue544',
            font: 'normal 24px "Material Icons"',
            textBaseline: 'bottom',
            fill: new Fill({
               color: 'black',
            })
         })
      });

      if (tracks != null) {
         for (let track of tracks) {
            if (null != track && null != track.Locations) {
               features.push(MapComponent.featureFromLocations(track.Locations));
            }
         }
      }

      return new VectorLayer({
         source: new VectorSource({
            features: features,
         }),
         style: [
            new Style({
               stroke: new Stroke({
                  color: 'blue',
                  width: 3,
               }),
            })
         ]
      });
   }

   protected static getPOILayer(pois: PointOfInterest[] | null): BaseLayer {
      var features: Feature[] = [];

      const parkingStyle = new Style({
         text: new Text({
            text: '\ue54f',
            font: 'normal 24px "Material Icons"',
            textBaseline: 'bottom',
            fill: new Fill({
               color: 'black',
            })
         })
      });
      const waterStyle = new Style({
         text: new Text({
            text: '\ue544',
            font: 'normal 24px "Material Icons"',
            textBaseline: 'bottom',
            fill: new Fill({
               color: 'black',
            })
         })
      });

      if (pois != null) {
         for (let poi of pois) {
            var f = MapComponent.featureFromLocation(poi.Location);
            let style = poi.Type == 'parking' ? parkingStyle : waterStyle;
            f.setStyle(style);
            features.push(f);
         }
      }

      return new VectorLayer({
         source: new VectorSource({
            features: features,
         })
      });
   }

   protected static getAreaBorderLayer(locations: Location[] | null): BaseLayer {
      var points: Coordinate[] = [];

      if (locations != null) {
         for (var loc of locations) {
            points.push(MapComponent.fromLocation(loc));
         }
      }

      return new VectorLayer({
         source: new VectorSource({
            features: [
               new Feature({
                  geometry: new Polygon([points]),
                  name: 'area'
               })
            ],
         }),
         style: [
            new Style({
               stroke: new Stroke({
                  color: 'blue',
                  width: 3,
               }),
               fill: new Fill({
                  color: 'rgba(0, 0, 255, 0.1)',
               }),
            })
         ],
      });
   }

   protected static featureFromLocation(location: Location): Feature {
      return new Feature({
         geometry: new Point(MapComponent.fromLocation(location)),
      });
   }

   protected static featureFromLocations(locations: Location[]): Feature {
      let track: Coordinate[] = [];

      for (let loc of locations) {
         track.push(MapComponent.fromLocation(loc));
      }

      return new Feature({
         geometry: new LineString(track),
      });
   }

   protected static fromLocation(location: Location): Coordinate {
      return transform(
         [location.Longitude, location.Latitude],
         'EPSG:4326',
         MapComponent.projectionId);
   }

   protected static fromLonLat(longitude: number, latitude: number): Coordinate {
      return MapComponent.fromLocation(new Location(longitude, latitude));
   }

   protected static radToDeg(rad: number): number {
      return (rad * 360) / (Math.PI * 2);
   }
}
