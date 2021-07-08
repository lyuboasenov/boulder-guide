import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AreaInfo } from './domain/AreaInfo';
import { Region } from './domain/Region';
import { RouteInfo } from './domain/RouteInfo';
import { Globals } from './globals';

@Component({
   selector: 'app-root',
   templateUrl: './app.component.html',
   styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit {
   title = 'Boulder Guide';
   areas: any[] = [];
   counter: number = 0;

   constructor(private http: HttpClient, private globals: Globals) { }

   ngOnInit() {
      this.getMasterIndex();
   }

   getMasterIndex() {
      this.http.get<Region[]>(environment.masterIndex).subscribe(
         (regions: Region[]) => this.initializeRegions(regions)
      )
   }

   initializeRegions(regions: Region[]) {
      for (var region of regions) {
         if (region.access == 'public' && !region.encrypted) {
            this.counter++;
            let r: Region = region;
            var url = region.url.replace(environment.apiRootPath, '/api') + '/index.json';
            this.http.get<AreaInfo>(url).subscribe(
               a => this.initializeArea(r, a),
               e => this.finalizeAreaCollection()
            );
         }
      }
   }

   initializeArea(r: Region, a: AreaInfo) {
      a = this.updateUrls(r, a);
      this.areas.push(a);
      this.finalizeAreaCollection();
   }

   updateUrls(r: Region, a: AreaInfo): AreaInfo {
      a.index = r.url + a.index;
      if (a.images != null) {
         for (var i = 0; i < a.images.length; i++) {
            a.images[i] = r.url + a.images[i];
         }
      }

      if (a.areas != null) {
         for (var i = 0; i < a.areas.length; i++) {
            a.areas[i] = this.updateUrls(r, a.areas[i]);
         }
      }

      if (a.routes != null) {
         for (var i = 0; i < a.routes.length; i++) {
            a.routes[i] = this.updateRouteUrls(r, a.routes[i]);
         }
      }

      return a;
   }

   updateRouteUrls(r: Region, route: RouteInfo): RouteInfo {
      route.index = r.url + route.index;

      if (route.images != null) {
         for (var i = 0; i < route.images.length; i++) {
            route.images[i] = r.url + route.images[i];
         }
      }

      return route;
   }

   finalizeAreaCollection() {
      this.counter--;

      if (this.counter == 0) {
         this.globals.areas = this.areas;
      }
   }
}
