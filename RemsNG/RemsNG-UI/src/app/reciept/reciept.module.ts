import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { UserModule } from "../user/user.module";
import { RecieptComponent } from './components/reciept.component';
import { RecieptService } from './services/reciept.service';

const appRoutes: Routes = [
    { path: 'reciept', component: RecieptComponent }
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
        RecieptComponent
    ],
    providers: [ RecieptService],
    exports: [
        RecieptComponent
    ]
  })

export class RecieptModule {

}
