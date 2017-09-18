import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { LcdaComponent } from './components/lcda.component';
import { LcdaService } from './services/lcda.services';

const appRoutes: Routes = [
    { path: 'lcda', component: LcdaComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule,
      ReactiveFormsModule,
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
