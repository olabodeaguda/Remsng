import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {DashboardComponent} from './components/dashboard.component';
import {SharedModule} from '.././shared/shared.module';
import { DashboardIndexComponent } from './components/dashboard-index.component';
import { CarouselModule } from 'angular4-carousel';
import { ChartsModule } from 'ng2-charts';

const appRoutes: Routes = [
     { path: 'dashboard', component: DashboardComponent }
 ];

@NgModule({
    imports: [
      BrowserModule, ChartsModule,
      SharedModule, CarouselModule,
      RouterModule.forChild(appRoutes), SharedModule
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
