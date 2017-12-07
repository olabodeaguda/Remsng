import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {WardComponent} from './components/ward.component';
import { LaddaModule } from 'angular2-ladda';
import { LoginService } from '../shared/services/login.service';
import { WardService } from './services/ward.service';
import { SharedModule } from '../shared/shared.module';

const appRoutes: Routes = [
    { path: 'ward/:id', component: WardComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule, SharedModule,
      ReactiveFormsModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        WardComponent
    ],
    providers: [ WardService],
    exports: [
        WardComponent
    ]
  })

export class WardModule {

}
