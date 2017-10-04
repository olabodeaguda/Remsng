import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { UserComponent } from './components/user.component';
import { UserService } from './services/user.service';
import { UserProfileComponent } from "./components/user-profile.component";

const appRoutes: Routes = [
    { path: 'users', component: UserComponent },
    { path: 'profile/:id', component: UserComponent }
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
        UserComponent, UserProfileComponent
    ],
    providers: [ UserService],
    exports: [
        UserComponent
    ]
  })

export class UserModule {

}
