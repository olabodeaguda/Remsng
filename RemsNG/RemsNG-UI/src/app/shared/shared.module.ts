import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {HeaderComponent} from './components/header.component';

const appRoutes: Routes = [
   // { path: 'dashboard', component: HeaderComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        HeaderComponent
    ],
    providers: [ ],
    exports: [
        HeaderComponent
    ]
  })

export class SharedModule {

}