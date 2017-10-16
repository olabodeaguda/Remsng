import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { ItemService } from "./services/item.service";
import { ItemComponent } from "./components/item.component";

const appRoutes: Routes = [
    { path: 'item/:id', component: ItemComponent }
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
        ItemComponent
    ],
    providers: [ ItemService],
    exports: [
        ItemComponent
    ]
  })

export class ItemModule {

}
