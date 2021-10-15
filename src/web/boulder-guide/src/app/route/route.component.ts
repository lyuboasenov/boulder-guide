import { Component, OnInit, Input, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { Area } from '../domain/Area';
import { Route } from '../domain/Route';
import { RouteInfo } from '../domain/RouteInfo';
import { RouteMapComponent } from '../maps/route-map.component';
import { DataService } from '../services/data.service';
import { NgbCarousel, NgbSlideEvent, NgbSlideEventSource } from '@ng-bootstrap/ng-bootstrap';
import { AreaInfo } from '../domain/AreaInfo';

@Component({
   selector: 'bg-route',
   templateUrl: './route.component.html',
   styleUrls: [ './route.component.scss' ],
   encapsulation: ViewEncapsulation.None
})

export class RouteComponent implements OnInit {

   @Input() info?: RouteInfo;

   @ViewChild('carousel', {static : true}) carousel!: NgbCarousel;
   @ViewChild(RouteMapComponent)
   private map!: RouteMapComponent;

   route?: Route | null;
   area?: Area | null;
   path: AreaInfo[] = [];

   paused = false;
   unpauseOnArrow = false;
   pauseOnIndicator = false;
   pauseOnHover = true;
   pauseOnFocus = true;

   constructor(private dataService: DataService) { }

   async ngOnInit() {
      if (this.info != null) {
         this.route = await this.dataService.getRoute(this.info);
         this.area = await this.dataService.getArea(this.info.areaInfo);

         let a = this.info.areaInfo;
         while (a) {
            this.path.push(a);
            a = a.areaInfo;
         }

         this.path = this.path.reverse();
      }
   }

   onTabChange(event: MatTabChangeEvent) {
      if (event.tab.ariaLabel && event.tab.ariaLabel === 'map-tab') {
         this.map.initMap();
      } else if (event.tab.ariaLabel && event.tab.ariaLabel === 'video-tab') {
         let instgrm = (window as { [key: string]: any })['instgrm'];
         if (instgrm) {
            instgrm.Embeds.process();
         }
      }
   }

   onMapReady(event: any) {
      console.log("Map Ready");
   }

   onSlide(slideEvent: NgbSlideEvent) {
      if (this.unpauseOnArrow && slideEvent.paused &&
         (slideEvent.source === NgbSlideEventSource.ARROW_LEFT || slideEvent.source === NgbSlideEventSource.ARROW_RIGHT)) {
         this.togglePaused();
      }
      if (this.pauseOnIndicator && !slideEvent.paused && slideEvent.source === NgbSlideEventSource.INDICATOR) {
         this.togglePaused();
      }
   }

   togglePaused() {
      if (this.paused) {
        this.carousel.cycle();
      } else {
        this.carousel.pause();
      }
      this.paused = !this.paused;
   }
}
