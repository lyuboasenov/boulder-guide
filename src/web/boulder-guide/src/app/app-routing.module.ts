import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AreaComponent } from './area/area.component';
import { HomeComponent } from './home/home.component';
import { MapComponent } from './map/map.component';
import { RouteComponent } from './route/route.component';

import { TopoViewComponent } from './topo.view/topo.view.component';

const routes: Routes = [
   { path: '', redirectTo: '/home', pathMatch: 'full' },
   { path: 'home', component: HomeComponent },
   { path: 'area/:id', component: AreaComponent },
   { path: 'route/:id', component: RouteComponent },
   { path: 'topo', component: TopoViewComponent },
   { path: 'map', component: MapComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
