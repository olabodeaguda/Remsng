import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { RoleComponent } from './components/role.component';
import { RoleService } from './services/role.service';
import { PermissionComponent } from './components/permission.component';
import { PermissionService } from './services/permission.service';

const appRoutes: Routes = [
    { path: 'role', component: RoleComponent },
    { path: 'permission/:id', component: PermissionComponent }
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
        RoleComponent,PermissionComponent
    ],
    providers: [ RoleService, PermissionService],
    exports: [
        RoleComponent, PermissionComponent
    ]
  })

export class RoleModule {

}
