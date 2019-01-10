import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { TaxPayerComponent } from "./components/taxpayer.component";
import { TaxpayerService } from "./services/taxpayer.service";
import { TaxPayerGlobalComponent } from './components/taxpayer-global.component';
import { TaxpayerPayerHistory } from './components/taxpayer.payment.component';
import { TaxpayerPayableComponent } from './components/taxpayer.payable.component';

const appRoutes: Routes = [
    { path: 'taxpayers/:id', component: TaxPayerComponent },
    { path: 'taxpayersglobal/:id', component: TaxPayerGlobalComponent },
    { path: 'paymentHistory/:id', component: TaxpayerPayerHistory },
    { path: 'paymentpayable/:id', component: TaxpayerPayableComponent }
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
        TaxPayerComponent,TaxPayerGlobalComponent,
         TaxpayerPayerHistory, TaxpayerPayableComponent
    ],
    providers: [ TaxpayerService],
    exports: [
        TaxPayerComponent,TaxPayerGlobalComponent, 
        TaxpayerPayerHistory, TaxpayerPayableComponent
    ]
  })

export class TaxPayersModule {

}
