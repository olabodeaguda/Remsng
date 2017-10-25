import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { ItemPenaltyModule } from "../item-penalty/itempenalty.module";
import { ComponentItemComponent } from "./components/company-item.component";
import { ComponentItemService } from "./services/company-item.service";

const appRoutes: Routes = [
    { path: 'companyitem/:id', component: ComponentItemComponent }
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
        ComponentItemComponent
    ],
    providers: [ ComponentItemService],
    exports: [
        ComponentItemComponent
    ]
  })

export class CompanyItemModule {

}
