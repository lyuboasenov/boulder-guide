import { Component, NgZone, Output, Input, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { Map } from 'ol';
import { Location } from '../domain/Location';
import BaseLayer from 'ol/layer/Base';
import { Area } from '../domain/Area';
import { MapComponent } from './map.component';
import { Route } from '../domain/Route';
import { RouteInfo } from '../domain/RouteInfo';


@Component({
   selector: 'bg-route-map',
   templateUrl: './map.component.html',
   styleUrls: ['./map.component.scss']
})
export class RouteMapComponent extends MapComponent {

   @Input() area!: Area;
   @Input() route!: Route;
   @Input() info!: RouteInfo;

   @Output() mapReady = new EventEmitter<Map>();

   constructor(zone: NgZone, cd: ChangeDetectorRef) {
      super(zone, cd);
   }

   public initMap(): void {
      super.initMap();
      setTimeout(() => this.mapReady.emit(this.map));
   }

   protected getAdditionalLayers(): BaseLayer[] {
      return [
         MapComponent.getAreaBorderLayer(this.area.Location),
         MapComponent.getRoutesLayer([ this.info ]),
         MapComponent.getPOILayer(this.area.POIs),
         MapComponent.getTrackLayer(this.area.Tracks)
      ];
   }

   protected getMapBorder(): Location[] {
      return this.area.Location;
   }
}
