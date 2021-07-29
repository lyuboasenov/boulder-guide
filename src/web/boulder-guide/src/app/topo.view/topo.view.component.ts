import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';

import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

import * as paths from './path';
import { Route } from '../domain/Route';

@Component({
   selector: 'topo.view',
   templateUrl: './topo.view.component.html',
   styleUrls: ['./topo.view.component.scss']
})

export class TopoViewComponent implements OnInit {
   @ViewChild('img') img!: ElementRef<HTMLImageElement>;
   paths: any[] = [];
   circles: any[] = [];
   ellipses: any[] = [];
   imgUrl: string = 'https://storage.googleapis.com/boulder-maps/map-vitosha-dev/main/boyana/myrtva_tyaga_0.jpg';
   url: string = '/api/boulder-maps/map-vitosha-dev/main/boyana/myrtva_tyaga.json';
   route!: Route | null;
   imgWidth: number = 0;
   imgHeight: number = 0;

   constructor(private http: HttpClient) { }

   ngOnInit() {
      this.getRoute().subscribe(
         r => { this.route = r; this.initialize(); }
      )
   }

   initialize(): void {
      this.paths = [];
      this.ellipses = [];
      if (this.route != null && this.imgWidth > 0 && this.imgHeight > 0) {
         for (var shape of this.route.Schemas[0].Shapes) {
            if (shape._type == 'Path') {
               this.addPath(shape);
            } else if (shape._type == 'Ellipse') {
               this.addEllipse(shape);
            }
         }
      }
   }

   addEllipse(shape: any) {
      this.ellipses.push({
         cx: Math.trunc(shape.Center.X * this.imgWidth),
         cy: Math.trunc(shape.Center.Y * this.imgHeight),
         rx: Math.trunc(Math.abs(shape.Center.X - shape.Radius.X) * this.imgWidth),
         ry: Math.trunc(Math.abs(shape.Center.Y - shape.Radius.Y) * this.imgHeight),
      })
   }

   addPath(shape: any) {
      var points: paths.Point[] = [];
      for (var p of shape.Points) {
         points.push({ x: Math.trunc(p.X * this.imgWidth), y: Math.trunc(p.Y * this.imgHeight) });
      }

      var pathD: string = '';
      for (var item of paths.calculatePath(points)) {
         pathD = pathD.concat(item.l);
         for (var cp of item.points) {
            pathD = pathD.concat(Math.trunc(cp.x) + ',' + Math.trunc(cp.y) + ' ');
         }
         pathD = pathD.concat(item.s + ' ');
      }
      this.paths.push(pathD);
   }

   getRoute() : Observable<Route> {
      return this.http.get<Route>(this.url);
   }

   onLoad() {
      this.imgWidth = (this.img.nativeElement as HTMLImageElement).clientWidth;
      this.imgHeight = (this.img.nativeElement as HTMLImageElement).clientHeight;
      this.initialize();
   }
}
