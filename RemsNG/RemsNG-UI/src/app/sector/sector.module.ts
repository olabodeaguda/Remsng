import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { SectorComponent } from './components/sector.component';
import { SectorService } from './services/sector.services';

const appRoutes: Routes = [
    { path: 'sector/:id', component: SectorComponent }
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
        SectorComponent
    ],
    providers: [ SectorService],
    exports: [
        SectorComponent
    ]
  })

export class SectorModule {
}
