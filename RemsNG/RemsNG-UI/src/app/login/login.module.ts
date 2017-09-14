import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './components/login.component';
import { LaddaModule } from 'angular2-ladda';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule,
      NgbModule.forRoot(),
      ReactiveFormsModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        LoginComponent
    ],
    providers: [ ],
    exports: [
        LoginComponent
    ]
  })

export class LoginModule {

}
