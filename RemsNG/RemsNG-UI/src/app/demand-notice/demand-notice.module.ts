import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { ItemPenaltyModule } from "../item-penalty/itempenalty.module";
import { DemandNoticeService } from "./services/demand-notice.service";
import { DemandNoticeComponent } from "./components/demand-notice.component";

const appRoutes: Routes = [
    { path: 'demandnotice', component: DemandNoticeComponent }
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
        DemandNoticeComponent
    ],
    providers: [ DemandNoticeService],
    exports: [
        DemandNoticeComponent
    ]
  })

export class DemandNoticeModule {

}
