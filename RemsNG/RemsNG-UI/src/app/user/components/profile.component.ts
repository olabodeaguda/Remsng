import { Component, Input, OnInit } from '@angular/core';
import { UserService } from "../services/user.service";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { ProfileModel } from "../models/profile.model";

@Component({
    selector: 'user-profile',
    templateUrl: '../views/profile.component.html'
})

export class ProfileComponent {

    @Input() profileModel: ProfileModel;

    isLoading: boolean = false;
    isDisabled: boolean = true;

    constructor(private userService: UserService, private toasterService: ToasterService) {
       this.profileModel = new ProfileModel();
        this.profileModel.eventType = "Edit";
    }

    toggle() {
        //disable all form or enable all form
        this.isDisabled = !this.isDisabled;
        if (this.isDisabled) {
            this.profileModel.eventType = "Edit";
        } else {
            this.profileModel.eventType = "Cancel";
        }
    }

    update() {
        this.isLoading = true;
        this.userService.update(this.profileModel).subscribe(response => {
            this.isLoading = false;
            const respD = Object.assign(new ResponseModel(), response.json());
            if(respD.code == '00'){
                this.toasterService.pop('success','Success',respD.description)
                this.toggle();
            } else{
                this.toasterService.pop('error','Error',respD.description)
            }
        }, error => {
            this.isLoading = false;
        })
    }

}