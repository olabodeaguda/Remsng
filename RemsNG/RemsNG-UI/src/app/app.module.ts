import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/components/login.component';
import { LoginModule } from './login/login.module';
import { DashboardComponent } from './Dashboard/components/dashboard.component';
import { DashBoardModule } from './Dashboard/dashboard.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from './shared/shared.module';
import { LaddaModule } from 'angular2-ladda';
import {ToasterModule, ToasterService} from 'angular2-toaster';
import { AppSettings } from './shared/models/app.settings';
import { DataService } from './shared/services/data.service';
import { StorageService } from './shared/services/storage.service';
import { DomainModule } from './domain/domain.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LCDAModule } from './lcda/lcda.module';
import { WardModule } from './ward/ward.module';
import { UserModule } from './user/user.module';


const appRoutes: Routes = [
    { path: '', component: LoginComponent }
 ];

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
        LoginModule,
        DashBoardModule,
        FormsModule,
        DomainModule,
        WardModule, UserModule,
        ToasterModule, LCDAModule,
        BrowserAnimationsModule,
        LaddaModule.forRoot({
            style: 'zoom-in',
            spinnerSize: 25,
            spinnerColor: 'green',
            spinnerLines: 12
        }),
        ReactiveFormsModule,
        SharedModule,
        RouterModule.forRoot(appRoutes, {useHash: true})
    ],
    providers: [ToasterService, AppSettings, DataService, StorageService],
    bootstrap: [AppComponent],
    exports: []
})
export class AppModule { }
