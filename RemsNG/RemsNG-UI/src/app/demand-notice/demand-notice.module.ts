import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { ItemPenaltyModule } from "../item-penalty/itempenalty.module";
import { DemandNoticeService } from "./services/demand-notice.service";
import { DemandNoticeComponent } from "./components/demand-notice.component";
import { DemandNoticeTaxpayersComponent } from './components/demand-noticeTaxpayers.component';
import { DemandNoticeTaxpayerService } from './services/demand-noticeTaxpayer.service';
import { DemandNoticeIndexComponent } from './components/demand-notice-index.component';
import { DemandNoticeSearchComponent } from './components/demand-notice-search.component';

const appRoutes: Routes = [
    { path: 'demandnotice', component: DemandNoticeIndexComponent,
        children:[
            { path: '', component: DemandNoticeComponent, pathMatch: 'full' },
            { path: 'taxpayer/:batchId', component: DemandNoticeTaxpayersComponent, pathMatch: 'full' },            
            { path: 'searchtaxpayer', component: DemandNoticeSearchComponent }
        ]
    },
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule,
      ReactiveFormsModule,
      SharedModule,
      ItemPenaltyModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        DemandNoticeComponent,DemandNoticeTaxpayersComponent,
        DemandNoticeIndexComponent,DemandNoticeSearchComponent
    ],
    providers: [ DemandNoticeService, DemandNoticeTaxpayerService],
    exports: [
        DemandNoticeComponent,DemandNoticeTaxpayersComponent,
        DemandNoticeIndexComponent,DemandNoticeSearchComponent
    ]
  })

export class DemandNoticeModule {

}
