import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import { LoginService } from '../shared/services/login.service';
import { SharedModule } from '../shared/shared.module';
import { TaxPayerComponent } from "./components/taxpayer.component";
import { TaxpayerService } from "./services/taxpayer.service";
import { StreetModule } from "../street/street.module";

const appRoutes: Routes = [
    { path: 'taxpayers/:id', component: TaxPayerComponent }
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
        TaxPayerComponent
    ],
    providers: [ TaxpayerService],
    exports: [
        TaxPayerComponent
    ]
  })

export class TaxPayersModule {

}
