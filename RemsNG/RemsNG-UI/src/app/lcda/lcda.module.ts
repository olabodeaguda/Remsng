import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { LcdaComponent } from './components/lcda.component';
import { LcdaService } from './services/lcda.services';
import { SharedModule } from '../shared/shared.module';
import { ItemModule } from "../items/item.module";
import { SectorModule } from "../sector/sector.module";
import { CategoryModule } from "../Category/category.module";
import { ItemPenaltyModule } from "../item-penalty/itempenalty.module";
import { CompanyModule } from "../company/company.module";

const appRoutes: Routes = [
    { path: 'lcda', component: LcdaComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule,
      ReactiveFormsModule,ItemModule,CategoryModule,
      SharedModule,SectorModule,ItemPenaltyModule,CompanyModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        LcdaComponent
    ],
    providers: [ LcdaService],
    exports: [
        LcdaComponent
    ]
  })

export class LCDAModule {

}
