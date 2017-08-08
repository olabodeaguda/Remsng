import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AlertModule } from 'ngx-bootstrap';
import { AppComponent } from './app.component';
import { LoginModule } from './login/login.module';
import { DashboardComponent } from './Dashboard/components/dashboard.component';
import { DashBoardModule } from './Dashboard/dashboard.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from './shared/shared.module';

const appRoutes: Routes = [
    {
        path: '', component: DashboardComponent
    }
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
        ReactiveFormsModule,
        SharedModule,
        RouterModule.forChild(appRoutes)
    ],
    providers: [],
    bootstrap: [AppComponent],
    exports: []
})
export class AppModule { }
