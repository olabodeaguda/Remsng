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
import { ToasterModule, ToasterService } from 'angular2-toaster';
import { AppSettings } from './shared/models/app.settings';
import { DataService } from './shared/services/data.service';
import { StorageService } from './shared/services/storage.service';
import { DomainModule } from './domain/domain.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LCDAModule } from './lcda/lcda.module';
import { WardModule } from './ward/ward.module';
import { UserModule } from './user/user.module';
import { RoleModule } from './role/role.module';
import { StreetModule } from './street/street.module';
import { ItemModule } from './items/item.module';
import { CategoryModule } from './Category/category.module';
import { ItemPenaltyModule } from './item-penalty/itempenalty.module';
import { TaxPayersModule } from './taxpayers/taxpayer.module';
import { CompanyModule } from './company/company.module';
import { DemandNoticeModule } from './demand-notice/demand-notice.module';
import { CompanyItemModule } from './companyitems/companyitem.module';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { GlobalInterceptorService } from './shared/services/global-interceptor.service';
import {HttpClientModule} from '@angular/common/http';
import { MediaFilesModule } from './media-files/media-files.module';
import { AddressGlobalModule } from './address/AddressGlobal.module';
import { DashboardIndexComponent } from './Dashboard/components/dashboard-index.component';
import { RecieptModule } from './reciept/reciept.module';
import { ReportModule } from './report/report.module';
// import { MyDatePickerModule } from 'angular4-datepicker';

const appRoutes: Routes = [
    { path: '', component: DashboardIndexComponent }
];

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
        LoginModule, ReportModule,
        DashBoardModule, RecieptModule,
        FormsModule, MediaFilesModule, AddressGlobalModule,
        DomainModule, HttpClientModule,
        WardModule, UserModule, StreetModule, CategoryModule,
        ToasterModule, LCDAModule, RoleModule, ItemPenaltyModule, CompanyItemModule,
        BrowserAnimationsModule, ItemModule, TaxPayersModule, CompanyModule,
        DemandNoticeModule,
        LaddaModule.forRoot({
            style: 'zoom-in',
            spinnerSize: 25,
            spinnerColor: 'green',
            spinnerLines: 12
        }),
        ReactiveFormsModule,
        SharedModule,
        RouterModule.forRoot(appRoutes, { useHash: true })
    ],
    providers: [ToasterService, AppSettings, DataService, StorageService, {
        provide: HTTP_INTERCEPTORS,
        useClass: GlobalInterceptorService,
        multi: true,
    }],
    bootstrap: [AppComponent],
    exports: []
})

export class AppModule { }
