import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { CommonModule } from '@angular/common';
import { TopoViewComponent } from './topo.view/topo.view.component';
import { MapComponent } from './map/map.component';
import { OlMapComponent } from './ol-map/ol-map.component';
import { HomeComponent } from './home/home.component';
import { AreaComponent } from './area/area.component';
import { RouteComponent } from './route/route.component';
import { AppComponent } from './app.component';
import { ViewComponent } from './view/view/view.component';
import { DataService } from './services/data.service';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AreaComponent,
    RouteComponent,
    TopoViewComponent,
    MapComponent,
    OlMapComponent,
    AppComponent,
    ViewComponent
  ],
  imports: [
    CommonModule,
    BrowserModule,
    HttpClientModule,
    AppRoutingModule
  ],
  providers: [ DataService ],
  bootstrap: [AppComponent]
})
export class AppModule { }
