import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {DashboardComponent} from './components/dashboard.component';

const appRoutes: Routes = [
    { path: 'dashboard', component: DashboardComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        DashboardComponent
    ],
    providers: [ ],
    exports: [
        DashboardComponent
    ]
  })

export class DashBoardModule {

}