import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AlertModule } from 'ngx-bootstrap';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/components/login.component';
import { LoginModule } from './login/login.module';
import { DashboardComponent } from './Dashboard/components/dashboard.component';
import { DashBoardModule } from './Dashboard/dashboard.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from './shared/shared.module';
import { LaddaModule } from 'angular2-ladda';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';

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
        NgbModule,
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
    providers: [],
    bootstrap: [AppComponent],
    exports: []
})
export class AppModule { }
