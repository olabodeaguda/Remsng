import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './components/login.component';
import { LaddaModule } from 'angular2-ladda';
import { LoginService } from '../shared/services/login.service';

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule,
      ReactiveFormsModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        LoginComponent
    ],
    providers: [ LoginService],
    exports: [
        LoginComponent
    ]
  })

export class LoginModule {

}
