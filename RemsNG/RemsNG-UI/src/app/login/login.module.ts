import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {LoginComponent} from './components/login.component';

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
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