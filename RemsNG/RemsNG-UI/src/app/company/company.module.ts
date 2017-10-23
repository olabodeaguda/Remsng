import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import { LoginService } from '../shared/services/login.service';
import { SharedModule } from '../shared/shared.module';
import { StreetModule } from "../street/street.module";
import { CompanyComponent } from "./components/company.component";
import { CompanyService } from "./services/company.service";
import { CompanyProfileComponent } from "./components/company-profile.component";
import { CoyMiniProfileComponent } from "./components/coymini-profile.component";
import { UserModule } from "../user/user.module";

const appRoutes: Routes = [
    { path: 'company/:id', component: CompanyComponent },
    { path: 'companyprofile/:id/:lcdaId', component: CompanyProfileComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule, SharedModule,UserModule,
      NgbModule.forRoot(),
      ReactiveFormsModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        CompanyProfileComponent,CompanyComponent,CoyMiniProfileComponent
    ],
    providers: [ CompanyService],
    exports: [
        CompanyComponent,CompanyProfileComponent,CoyMiniProfileComponent
    ]
  })

export class CompanyModule {

}
