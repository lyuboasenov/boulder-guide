import { Component, OnInit, Input, ViewChild  } from '@angular/core';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { Area } from '../domain/Area';
import { AreaInfo } from '../domain/AreaInfo';
import { AreaMapComponent } from '../maps/area-map.component';
import { DataService } from '../services/data.service';

@Component({
   selector: 'bg-area',
   templateUrl: './area.component.html',
   styleUrls: [ './area.component.scss' ]
})

export class AreaComponent implements OnInit {

   @Input() info?: AreaInfo;

   @ViewChild(AreaMapComponent)
   private map!: AreaMapComponent;

   area?: Area;
   isRoot: boolean = true;
   path: AreaInfo[] = [];

   constructor(private dataService: DataService) { }

   async ngOnInit() {
      if (this.info != null) {
         this.isRoot = this.info.id == this.info.rootId;
         var result = await this.dataService.getArea(this.info);
         if (result != null) {
            this.area = result;
         }

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
      }
   }

   onMapReady(event: any) {
      console.log("Map Ready");
   }
}
