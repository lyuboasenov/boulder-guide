import { Component, OnInit } from '@angular/core';
import { AreaInfo } from 'src/app/domain/AreaInfo';
import { DataService } from 'src/app/services/data.service';
import { ActivatedRoute, Router } from '@angular/router';
import { RouteInfo } from 'src/app/domain/RouteInfo';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.scss']
})
export class ViewComponent implements OnInit {
   areas: AreaInfo[] = [];
   area:any;
   routes: RouteInfo[] = [];
   id: string = '';

   constructor(
      private route: ActivatedRoute,
      private dataService: DataService,
      private router: Router) {
         this.router.routeReuseStrategy.shouldReuseRoute = function () {
            return false;
         };
      }

   ngOnInit(): void {
      var id = this.route.snapshot.paramMap.get('id');

      if (id == null || id == '') {
         this.id = '';
      } else {
         this.id = String(id) + '/'
      }

      this.dataService.getAreaInfo(this.id).then(a => {
         this.area = a;
         this.areas = a.areas;
      });
   }
}
