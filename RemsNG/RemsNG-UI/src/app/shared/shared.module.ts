import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import {HeaderComponent} from './components/header.component';
import {SideBarComponent} from './components/sideBar.component';
import {FooterComponent} from './components/footer.component';
import { DataService } from './services/data.service';
import { HttpModule } from '@angular/http';
import {ToasterModule, ToasterService} from 'angular2-toaster';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

const appRoutes: Routes = [
 ];

@NgModule({
    imports: [
      BrowserModule,
      ToasterModule,
      HttpModule,
      BrowserAnimationsModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        HeaderComponent, SideBarComponent, FooterComponent
    ],
    providers: [ToasterService],
    exports: [
        HeaderComponent, SideBarComponent, FooterComponent
    ]
  })

export class SharedModule {

}
