import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {HeaderComponent} from './components/header.component';
import {SideBarComponent} from './components/sideBar.component';
import {FooterComponent} from './components/footer.component';

const appRoutes: Routes = [
   // { path: 'dashboard', component: HeaderComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        HeaderComponent, SideBarComponent, FooterComponent
    ],
    providers: [ ],
    exports: [
        HeaderComponent, SideBarComponent, FooterComponent
    ]
  })

export class SharedModule {

}