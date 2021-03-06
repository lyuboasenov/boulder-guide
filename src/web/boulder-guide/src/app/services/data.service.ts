import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Area } from '../domain/Area';
import { AreaInfo } from '../domain/AreaInfo';
import { Region } from '../domain/Region';
import { RouteInfo } from '../domain/RouteInfo';
import { Route } from '../domain/Route';
import { returnOrUpdate } from 'ol/extent';

@Injectable({
providedIn: 'root'
})
export class DataService {
   private _areas: AreaInfo[] = [];
   private _initialized: boolean = false;
   private _initializing: boolean = false;

   constructor(private http: HttpClient) { }

   async getRoute(info: RouteInfo): Promise<Route|null> {
      if (info == null || info.index == null || info.index == '') {
         return new Promise<Route|null>((resolve) => { resolve(null); });
      }
      let baseUrl = info.index.substring(0, info.index.lastIndexOf('/') + 1);
      let route = await this.http.get<Route>(info.index).toPromise();
      if (route.Schemas != null) {
         for (let topo of route.Schemas) {
            topo.Id = baseUrl + topo.Id;
         }
      }

      return route;
   }

   async getArea(info: AreaInfo): Promise<Area|null> {
      if (info == null || info.index == null || info.index == '') {
         return new Promise<Area|null>((resolve) => { resolve(null); });
      }
      return this.http.get<Area>(info.index).toPromise();
   }

   async getAreaInfo(path: string): Promise<AreaInfo> {
      await this.initialize();

      var area!: AreaInfo;

      if (path == '') {
         return { rootId: '', id: '', areas: this._areas, name: '', index: '', routes: [], images: [], totalAreas: 0, totalRoutes: 0, areaInfo: area };
      } else {
         var ids = path.split('/').filter(s => s);
         var areas = this._areas;

         for (var i = 0; i < ids.length; i++) {
            var id = ids[i];
            if (areas != null) {
               for (var currentArea of areas) {
                  if (currentArea.id == id) {
                     if (i == ids.length - 1) {
                        area = currentArea;
                     }
                     areas = currentArea.areas;
                     break;
                  }
               }
            }
         }

         return area;
      }
   }

   async getRouteInfo(path: string): Promise<RouteInfo> {
      await this.initialize();

      var route!: RouteInfo;

      if (path == '') {
         return route;
      } else {
         var ids = path.split('/').filter(s => s);
         var areas = this._areas;
         var routes!: RouteInfo[];

         for (var i = 0; i < ids.length; i++) {
            var id = ids[i];
            if (i < ids.length -1 && areas != null) {
               for (var currentArea of areas) {
                  if (currentArea.id == id) {
                     if (i == ids.length - 2) {
                        routes = currentArea.routes;
                     }
                     areas = currentArea.areas;
                     break;
                  }
               }
            }

            if (i == ids.length - 1 && routes != null) {
               for (var currentRoute of routes) {
                  if (currentRoute.id == id) {
                     route = currentRoute;
                     break;
                  }
               }
            }
         }

         return route;
      }
   }

   async initialize() {
      if(!this._initialized && !this._initializing) {
         this._initializing = true;

         var regions = await this.http.get<Region[]>(environment.masterIndex).toPromise();
         this._areas = [];

         for (var region of regions) {
            if (region.access == 'public' && !region.encrypted) {
               var url = region.url + '/index.json';
               var areaInfo = await this.http.get<AreaInfo>(url).toPromise();
               areaInfo.rootId = '';
               areaInfo = this.build(region, areaInfo);
               this._areas.push(areaInfo);
            }
         }

         this._initializing = false;
         this._initialized = true;
      }

      while (this._initializing) {
         await this.delay(500);
      }
   }

   build(r: Region, a: AreaInfo): AreaInfo {
      a.rootId = a.rootId + '/' + a.id;
      a.index = r.url + a.index;
      if (a.images != null) {
         for (var i = 0; i < a.images.length; i++) {
            a.images[i] = r.url + a.images[i];
         }
      }

      var rCount: number = 0;
      var aCount: number = 0;

      if (a.areas != null) {
         for (var i = 0; i < a.areas.length; i++) {
            a.areas[i].rootId = a.rootId;
            a.areas[i].areaInfo = a;
            a.areas[i] = this.build(r, a.areas[i]);

            rCount += a.areas[i].totalRoutes;
            aCount += a.areas[i].totalAreas;
         }

         a.totalAreas = aCount + a.areas.length;
      } else {
         a.totalAreas = 0;
      }

      if (a.routes != null) {
         for (var i = 0; i < a.routes.length; i++) {
            a.routes[i].areaInfo = a;
            a.routes[i].rootId = a.rootId + '/' + a.routes[i].id;
            a.routes[i] = this.buildRoute(r, a.routes[i]);
         }

         a.totalRoutes = a.routes.length + rCount;
      } else {
         a.totalRoutes = rCount;
      }

      return a;
   }

   buildRoute(r: Region, route: RouteInfo): RouteInfo {
      route.index = r.url + route.index;

      if (route.images != null) {
         for (var i = 0; i < route.images.length; i++) {
            route.images[i] = r.url + route.images[i];
         }
      }

      return route;
   }

   delay(ms: number) {
      return new Promise( resolve => setTimeout(resolve, ms) );
  }
}
