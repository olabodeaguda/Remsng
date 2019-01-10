import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { LoginService } from '../shared/services/login.service';
import { SharedModule } from '../shared/shared.module';
import { TaxPayerComponent } from "./components/taxpayer.component";
import { TaxpayerService } from "./services/taxpayer.service";
import { StreetModule } from "../street/street.module";
import { TaxPayerGlobalComponent } from './components/taxpayer-global.component';
import { TaxpayerPayerHistory } from './components/taxpayer.payment.component';

const appRoutes: Routes = [
    { path: 'taxpayers/:id', component: TaxPayerComponent },
    { path: 'taxpayersglobal/:id', component: TaxPayerGlobalComponent },
    { path: 'paymentHistory/:id', component: TaxpayerPayerHistory }
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
        TaxPayerComponent,TaxPayerGlobalComponent, TaxpayerPayerHistory
    ],
    providers: [ TaxpayerService],
    exports: [
        TaxPayerComponent,TaxPayerGlobalComponent, TaxpayerPayerHistory
    ]
  })

export class TaxPayersModule {

}
