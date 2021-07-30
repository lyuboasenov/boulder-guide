import { Component, OnInit, Input, ViewChild  } from '@angular/core';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { Area } from '../domain/Area';
import { AreaInfo } from '../domain/AreaInfo';
import { OlMapComponent } from '../ol-map/ol-map.component';
import { DataService } from '../services/data.service';

@Component({
   selector: 'bg-area',
   templateUrl: './area.component.html',
   styleUrls: [ './area.component.scss' ]
})

export class AreaComponent implements OnInit {

   @Input() info?: AreaInfo;

   @ViewChild(OlMapComponent)
   private map!: OlMapComponent;

   area?: Area;
   isRoot: boolean = true;

   constructor(private dataService: DataService) { }

   async ngOnInit() {
      if (this.info != null) {
         this.isRoot = this.info.id == this.info.rootId;
         var result = await this.dataService.getArea(this.info);
         if (result != null) {
            this.area = result;
         }
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
