import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {DashboardComponent} from './components/dashboard.component';
import {SharedModule} from '.././shared/shared.module';
import { DashboardIndexComponent } from './components/dashboard-index.component';
import { CarouselModule } from 'angular4-carousel';
import { FusionChartsModule } from 'angular4-fusioncharts';
import * as FusionCharts from 'fusioncharts';
import * as Charts from 'fusioncharts/fusioncharts.charts';
import * as FintTheme from 'fusioncharts/themes/fusioncharts.theme.fint';

const appRoutes: Routes = [
     { path: 'dashboard', component: DashboardComponent }
 ];

FusionChartsModule.fcRoot(FusionCharts, Charts, FintTheme);

@NgModule({
    imports: [
      BrowserModule,
      SharedModule, CarouselModule,
      FusionChartsModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        DashboardComponent, DashboardIndexComponent
    ],
    providers: [ ],
    exports: [
        DashboardComponent, DashboardIndexComponent
    ]
  })

export class DashBoardModule {

}
