import { Component, OnInit, Input } from '@angular/core';
import { RouteInfo } from '../domain/RouteInfo';

@Component({
   selector: 'bg-route',
   templateUrl: './route.component.html',
   styleUrls: [ './route.component.scss' ]
})

export class RouteComponent implements OnInit {

   @Input() route?: RouteInfo;

   constructor() { }

   ngOnInit() { }
}
