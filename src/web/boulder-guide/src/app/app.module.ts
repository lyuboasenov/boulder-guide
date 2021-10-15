import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { CommonModule } from '@angular/common';
import { TopoComponent } from './topo/topo.component';
import { AreaMapComponent } from './maps/area-map.component';
import { RouteMapComponent } from './maps/route-map.component';
import { HomeComponent } from './home/home.component';
import { AreaComponent } from './area/area.component';
import { RouteComponent } from './route/route.component';
import { AppComponent } from './app.component';
import { ViewComponent } from './view/view.component';
import { DataService } from './services/data.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSliderModule } from '@angular/material/slider';
import { NavigationComponent } from './navigation/navigation.component';
import { LayoutModule } from '@angular/cdk/layout';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { GradePipe } from './pipes/grade.pipe';
import { MatTabsModule } from '@angular/material/tabs';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatStepperModule } from '@angular/material/stepper';
import { UrlPipe } from './pipes/url.pipe';
import { HtmlPipe } from './pipes/html.pipe';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AreaComponent,
    RouteComponent,
    TopoComponent,
    AreaMapComponent,
    RouteMapComponent,
    AppComponent,
    ViewComponent,
    NavigationComponent,
    GradePipe,
    UrlPipe,
    HtmlPipe
  ],
  imports: [
    CommonModule,
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    MatSliderModule,
    BrowserAnimationsModule,
    LayoutModule,
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatIconModule,
    MatListModule,
    MatCardModule,
    MatTabsModule,
    MatExpansionModule,
    MatStepperModule,
    NgbModule
  ],
  providers: [ DataService ],
  bootstrap: [ AppComponent ],
  schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
})
export class AppModule { }
