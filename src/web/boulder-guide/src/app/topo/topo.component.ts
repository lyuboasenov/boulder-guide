import { Component, ElementRef, OnInit, ViewChild, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import * as paths from './path';
import { Route } from '../domain/Route';
import { Topo } from '../domain/Topo';

@Component({
   selector: 'bg-topo',
   templateUrl: './topo.component.html',
   styleUrls: ['./topo.component.scss']
})

export class TopoComponent implements OnInit {

   @Input() topo?: Topo;

   @ViewChild('img') img!: ElementRef<HTMLImageElement>;

   paths: any[] = [];
   circles: any[] = [];
   ellipses: any[] = [];
   rectangles: any[] = [];
   imgUrl: string = 'https://storage.googleapis.com/boulder-maps/map-vitosha-dev/main/boyana/myrtva_tyaga_0.jpg';
   route!: Route | null;
   imgWidth: number = 0;
   imgHeight: number = 0;

   constructor() { }

   ngOnInit() {
      this.initialize();
   }

   initialize(): void {
      this.paths = [];
      this.ellipses = [];
      if (this.topo != null) {
         this.imgUrl = this.topo.Id;
         if (this.imgWidth > 0 && this.imgHeight > 0) {
            for (var shape of this.topo.Shapes) {
               if (shape._type == 'Path') {
                  this.addPath(shape);
               } else if (shape._type == 'Ellipse') {
                  this.addEllipse(shape);
               } else if (shape._type == 'Rectangle') {
                  this.addRectangle(shape);
               }
            }
         }
      }
   }

   addRectangle(shape: any) {
      let cx = Math.trunc(shape.Center.X * this.imgWidth);
      let cy = Math.trunc(shape.Center.Y * this.imgHeight);

      let width = Math.trunc(shape.Width * this.imgWidth);
      let height = Math.trunc(shape.Height * this.imgHeight);



      this.rectangles.push({
         x: cx - width / 2,
         y: cy - height / 2,
         width: width,
         height: height
      });
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

   onLoad() {
      this.imgWidth = (this.img.nativeElement as HTMLImageElement).clientWidth;
      this.imgHeight = (this.img.nativeElement as HTMLImageElement).clientHeight;
      this.initialize();
   }
}
