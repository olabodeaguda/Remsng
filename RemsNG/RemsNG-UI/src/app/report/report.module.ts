import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { ReportComponent } from './components/report.component';
import { ReportService } from './services/report.service';

const appRoutes: Routes = [
    { path: 'report', component: ReportComponent }
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
        ReportComponent
    ],
    providers: [ ReportService],
    exports: [
        ReportComponent
    ]
  })

export class ReportModule {
}
