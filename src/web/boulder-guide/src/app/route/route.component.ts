import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { Area } from '../domain/Area';
import { Route } from '../domain/Route';
import { RouteInfo } from '../domain/RouteInfo';
import { RouteMapComponent } from '../maps/route-map.component';
import { DataService } from '../services/data.service';

@Component({
   selector: 'bg-route',
   templateUrl: './route.component.html',
   styleUrls: [ './route.component.scss' ]
})

export class RouteComponent implements OnInit {

   @Input() info?: RouteInfo;

   @ViewChild(RouteMapComponent)
   private map!: RouteMapComponent;

   route?: Route | null;
   area?: Area | null;

   constructor(private dataService: DataService) { }

   async ngOnInit() {
      if (this.info != null) {
         this.route = await this.dataService.getRoute(this.info);
         this.area = await this.dataService.getArea(this.info.areaInfo);
      }
   }

   onTabChange(event: MatTabChangeEvent) {
      if (event.tab.ariaLabel && event.tab.ariaLabel === 'map-tab') {
         this.map.initMap();
      }

      let instgrm = (window as { [key: string]: any })['instgrm'];

      if (instgrm) {
         instgrm.Embeds.process();
         console.log(1);
       }
   }

   onMapReady(event: any) {
      console.log("Map Ready");
   }
}
