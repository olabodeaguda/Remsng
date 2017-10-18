import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { CategoryComponent } from "./components/category.component";
import { CategoryService } from "./services/category.service";

const appRoutes: Routes = [
    { path: 'category/:id', component: CategoryComponent }
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
        CategoryComponent
    ],
    providers: [ CategoryService],
    exports: [
        CategoryComponent
    ]
  })

export class CategoryModule {
}
