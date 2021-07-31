import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Route } from '../domain/Route';
import { RouteInfo } from '../domain/RouteInfo';
import { AreaMapComponent } from '../maps/area-map.component';
import { DataService } from '../services/data.service';

@Component({
   selector: 'bg-route',
   templateUrl: './route.component.html',
   styleUrls: [ './route.component.scss' ]
})

export class RouteComponent implements OnInit {

   @Input() info?: RouteInfo;

   @ViewChild(AreaMapComponent)
   private map!: AreaMapComponent;

   route?: Route;

   constructor(private dataService: DataService) { }

   async ngOnInit() {
      if (this.info != null) {
         var result = await this.dataService.getRoute(this.info);
         if (result != null) {
            this.route = result;
            console.log(this.route);
            console.log(this.route.Schemas);
         }
      }
   }
}
