import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { ItemPenaltyService } from "./services/item-penalty.service";
import { ItemPenaltyComponent } from "./components/item-penalty.component";

const appRoutes: Routes = [
    { path: 'itempenalty/:id', component: ItemPenaltyComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule,
      ReactiveFormsModule,
      SharedModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        ItemPenaltyComponent
    ],
    providers: [ ItemPenaltyService],
    exports: [
        ItemPenaltyComponent
    ]
  })

export class ItemPenaltyModule {

}
