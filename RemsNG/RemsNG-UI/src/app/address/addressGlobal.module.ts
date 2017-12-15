import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { AddressGlobalComponent } from './components/AddressGlobal.component';
import { AddressGlobalService } from './services/addressGlobal.service';

const appRoutes: Routes = [
    { path: 'address/:lcdaId/:ownerId', component: AddressGlobalComponent }
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
        AddressGlobalComponent
    ],
    providers: [ AddressGlobalService ],
    exports: [
        AddressGlobalComponent
    ]
  })

export class AddressGlobalModule {
}
