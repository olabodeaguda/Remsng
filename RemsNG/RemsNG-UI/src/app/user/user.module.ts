import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { UserComponent } from './components/user.component';
import { UserService } from './services/user.service';
import { UserProfileComponent } from "./components/user-profile.component";
import { ProfileComponent } from "./components/profile.component";
import { ContactComponent } from "./components/contact.component";
import { AddContactComponent } from "./components/add-contact.component";
import { ContactService } from "./services/contact.service";
import { RoleModule } from "../role/role.module";
import { AddressComponent } from "./components/address.component";
import { AddressService } from "./services/address.service";

const appRoutes: Routes = [
    { path: 'users', component: UserComponent },
    { path: 'user/:id', component: UserProfileComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule,
      ReactiveFormsModule,
      SharedModule,RoleModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        UserComponent, UserProfileComponent,AddressComponent,
        ProfileComponent,ContactComponent,AddContactComponent
    ],
    providers: [ UserService, ContactService, AddressService],
    exports: [
        UserComponent, UserProfileComponent, AddressComponent,
        ProfileComponent, ContactComponent,AddContactComponent
    ]
  })

export class UserModule {

}
