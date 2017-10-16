import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from '../shared/shared.module';
import { StreetComponent } from "./components/street.component";
import { StreetService } from "./services/street.service";

const appRoutes: Routes = [
    { path: 'street/:id', component: StreetComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule, SharedModule,
      NgbModule.forRoot(),
      ReactiveFormsModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        StreetComponent
    ],
    providers: [ StreetService],
    exports: [
        StreetComponent
    ]
  })

export class StreetModule {

}
