import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AreaInfo } from '../domain/AreaInfo';
import { Region } from '../domain/Region';
import { RouteInfo } from '../domain/RouteInfo';

@Injectable({
providedIn: 'root'
})
export class DataService {
   private _areas: AreaInfo[] = [];
   private _initialized: boolean = false;

   constructor(private http: HttpClient) { }

   async getAreaInfo(path: string): Promise<AreaInfo> {
      await this.initialize();

      if (path == '') {
         return { id: '', areas: this._areas, name: '', index: '', routes: [], images: [], totalAreas: 0, totalRoutes: 0 };
      } else {
         var ids = path.split('/');
         var areas = this._areas;
         var area!: AreaInfo;
         for (var id of ids) {
            if (areas != null) {
               for (var currentArea of areas) {
                  if (currentArea.id == id) {
                     area = currentArea;
                     areas = currentArea.areas;
                     break;
                  }
               }
            }
         }

         return area;
      }
   }

   async initialize() {
      if(!this._initialized) {
         var regions = await this.http.get<Region[]>(environment.masterIndex).toPromise();
         this._areas = [];

         for (var region of regions) {
            if (region.access == 'public' && !region.encrypted) {
               var url = region.url.replace(environment.apiRootPath, '/api') + '/index.json';
               var areaInfo = await this.http.get<AreaInfo>(url).toPromise();
               areaInfo = this.updateUrls(region, areaInfo);
               areaInfo = this.updateAreaRouteCounts(areaInfo);
               this._areas.push(areaInfo);
            }
         }
      }
   }

   updateAreaRouteCounts(a: AreaInfo): AreaInfo {
      var rCount: number = 0;
      var aCount: number = 0;

      if (a.areas != null) {
         for (var i = 0; i < a.areas.length; i++) {
            a.areas[i] = this.updateAreaRouteCounts(a.areas[i]);

            rCount += a.areas[i].totalRoutes;
            aCount += a.areas[i].totalAreas;
         }

         a.totalAreas = aCount + a.areas.length;
      } else {
         a.totalAreas = 0;
      }

      if (a.routes != null) {
         a.totalRoutes = a.routes.length + rCount;
      } else {
         a.totalRoutes = rCount;
      }

      return a;
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
}
