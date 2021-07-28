import { Component, OnInit, Input  } from '@angular/core';
import { AreaInfo } from '../domain/AreaInfo';

@Component({
   selector: 'app-area',
   templateUrl: './area.component.html',
   styleUrls: [ './area.component.scss' ]
})

export class AreaComponent implements OnInit {

   @Input() area?: AreaInfo;

   constructor() { }

   ngOnInit() { }
}
