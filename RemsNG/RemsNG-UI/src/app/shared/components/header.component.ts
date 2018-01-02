import { Component, Output, ElementRef, ViewChild } from '@angular/core';
import { UserModel } from '../models/user.model';
import { StorageService } from '../services/storage.service';
import { UserService } from '../../user/services/user.service';
import { ToasterService } from 'angular2-toaster';
import { ChangePasswordModel } from '../../user/models/change-password.model';
import { ResponseModel } from '../models/response.model';
import { ProfileModel } from '../../user/models/profile.model';
declare var jQuery: any;

@Component({
    selector: 'app-hd',
    templateUrl: '../views/header.component.html'
})

export class HeaderComponent {
    pm: ProfileModel = new ProfileModel();
    userModel: UserModel;
    changePwd: ChangePasswordModel = new ChangePasswordModel();
    @ViewChild('changepwd') changePwdModal: ElementRef;//
    constructor(private storageService: StorageService,
        private userService: UserService,
        private toasterService: ToasterService) {
        this.userModel = storageService.get();
        if (this.userModel == null) {
            this.userModel = new UserModel();
            this.storageService.remove();
        }
        storageService.usermodelEmit.subscribe((x) => {
            this.userModel = x;
        });
    }

    logout() {
        this.storageService.remove();
    }

    openChangePass(){
        this.userModel = this.storageService.get();
        if(this.userModel === null){
            return;
        }
        this.pm.id = this.userModel.id;
        jQuery(this.changePwdModal.nativeElement).modal('show');
    }

    changePasssword() {
        this.pm.isLoading = true;
        this.pm.id = this.userModel.id;
        this.userService.changePwd(this.pm, this.changePwd).subscribe(response => {
            this.pm.isLoading = false;
            const result = Object.assign(new ResponseModel(), response);
            if (result.code === '00') {
                this.toasterService.pop('success', 'Success', result.description);
                this.changePwd = new ChangePasswordModel();
                jQuery(this.changePwdModal.nativeElement).modal('hide');
            } else {
                this.toasterService.pop('error', 'Error', result.description);
            }
        },error=>{        
            this.toasterService.pop('error', 'Error', error);
            this.pm.isLoading = false;
        });
    }

}
