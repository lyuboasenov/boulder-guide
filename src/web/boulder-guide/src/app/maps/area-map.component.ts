import { Component, NgZone, Output, Input, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { View, Feature, Map } from 'ol';
import { Coordinate } from 'ol/coordinate';
import { ScaleLine, defaults as DefaultControls } from 'ol/control';
import proj4 from 'proj4';
import VectorLayer from 'ol/layer/Vector';
import { register } from 'ol/proj/proj4';
import { get as GetProjection, fromLonLat, transform } from 'ol/proj';
import { Extent } from 'ol/extent';
import TileLayer from 'ol/layer/Tile';
import OSM from 'ol/source/OSM';
import { Location } from '../domain/Location';
import BaseLayer from 'ol/layer/Base';
import VectorSource from 'ol/source/Vector';
import { Text, Style } from 'ol/style';
import Stroke from 'ol/style/Stroke';
import Fill from 'ol/style/Fill';
import Polygon from 'ol/geom/Polygon';
import { AreaInfo } from '../domain/AreaInfo';
import { Area } from '../domain/Area';
import Point from 'ol/geom/Point';


@Component({
   selector: 'bg-area-map',
   templateUrl: './area-map.component.html',
   styleUrls: ['./area-map.component.scss']
})
export class OlMapComponent {

   @Input() area!: Area;
   @Input() info!: AreaInfo;

   map!: Map;
   extent!: Extent;

   @Output() mapReady = new EventEmitter<Map>();

   readonly projectionId: string = "EPSG:3857";

   constructor(private zone: NgZone, private cd: ChangeDetectorRef) { }

   public initMap(): void {
      if (!this.map) {
         this.zone.runOutsideAngular(() => this.initMapInternal())
      }
      setTimeout(() => this.mapReady.emit(this.map));
   }

   private initMapInternal(): void {
      proj4.defs(this.projectionId, "+proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +wktext  +no_defs");
      register(proj4);

      this.map = new Map({
         layers: [
            this.getOSMLayer(),
            this.getAreaBorderLayer(),
            this.getRoutesLayer()
         ],
         target: 'map',
         view: this.getView(),
         controls: DefaultControls().extend([
            new ScaleLine({}),
         ]),
      });

      this.map.getView().fit(this.extent);
   }

   getView(): View {
      var projection = GetProjection(this.projectionId);

      var minLat: number = 1000;
      var maxLat: number = 0;
      var minLon: number = 1000;
      var maxLon: number = 0;

      for (var loc of this.area.Location) {
         minLat = Math.min(minLat, loc.Latitude);
         maxLat = Math.max(maxLat, loc.Latitude);
         minLon = Math.min(minLon, loc.Longitude);
         maxLon = Math.max(maxLon, loc.Longitude);
      }

      var minCoo = fromLonLat([minLon, minLat]);
      var maxCoo = fromLonLat([maxLon, maxLat]);
      this.extent = [minCoo[0], minCoo[1], maxCoo[0], maxCoo[1]];

      var centerLon = (minLon + maxLon) / 2;
      var centerLat = (minLat + maxLat) / 2;

      var center = fromLonLat([centerLon, centerLat]);

      return new View({
         center: center,
         zoom: 6,
         projection: projection,
      });
   }

   getOSMLayer(): BaseLayer {
      return new TileLayer({
         source: new OSM({})
      });
   }

   getAreaBorderLayer(): BaseLayer {
      var points: Coordinate[] = [];

      for (var loc of this.area.Location) {
         points.push(this.fromLocation(loc));
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

   getRoutesLayer(): BaseLayer {

      var features: Feature[] = [];

      if (this.info != null && this.info.routes != null) {
         for (var r of this.info.routes) {
            var f = this.featureFromLocation(r.location);
            f.setStyle(new Style({
               text: new Text({
                  text: r.name,
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
            features.push(this.featureFromLocation(r.location));
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

   featureFromLocation(location: Location): Feature {
      return new Feature({
         geometry: new Point(this.fromLocation(location)),
      });
   }

   fromLocation(location: Location): Coordinate {
      return transform([location.Longitude, location.Latitude], 'EPSG:4326', this.projectionId);
   }

   fromLonLat(longitude: number, latitude: number): Coordinate {
      return this.fromLocation({ Latitude: latitude, Longitude: longitude });
   }
}
