import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { StreetComponent } from "./components/street.component";
import { StreetService } from "./services/street.service";
import { TaxPayersModule } from "../taxpayers/taxpayer.module";

const appRoutes: Routes = [
    { path: 'street/:id', component: StreetComponent }
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
        StreetComponent
    ],
    providers: [ StreetService,TaxPayersModule],
    exports: [
        StreetComponent
    ]
  })

export class StreetModule {

}
