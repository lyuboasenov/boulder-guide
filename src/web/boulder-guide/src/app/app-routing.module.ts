import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MapComponent } from './map/map.component';

import { TopoViewComponent } from './topo.view/topo.view.component';

const routes: Routes = [
   { path: 'topo', component: TopoViewComponent },
   { path: 'map', component: MapComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
