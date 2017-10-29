import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { DomainComponent } from './components/domain.component';
import { SharedModule } from '../shared/shared.module';
import { DomainService } from './services/domain.service';
import { LaddaModule } from 'angular2-ladda';
import { AppModule } from "../app.module";

const appRoutes: Routes = [
    { path: 'domain', component: DomainComponent }
];

@NgModule({
   imports: [
     BrowserModule,
     FormsModule,
     LaddaModule,
     SharedModule,
     RouterModule.forChild(appRoutes)
   ],
   declarations: [
    DomainComponent
   ],
   providers: [DomainService],
   exports: [
    DomainComponent
   ]
 })

export class DomainModule {

}
