import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {DashboardComponent} from './components/dashboard.component';
import {SharedModule} from '.././shared/shared.module';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';

const appRoutes: Routes = [
     { path: 'dashboard', component: DashboardComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      SharedModule,
      NgbModule.forRoot(),
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
