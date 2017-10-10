import {Component, OnInit} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { UserService } from "../services/user.service";
import { ProfileModel } from "../models/profile.model";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";

@Component({
    selector:'app-userprofile',
    templateUrl: '../views/user-profile.component.html'
})

export class UserProfileComponent implements OnInit{    
    profileModel:ProfileModel;
    
    ngOnInit(): void {
        this.initializePage();
    }

    constructor(private activeRoute: ActivatedRoute, 
        private userService: UserService,  private toasterService: ToasterService) {
        this.profileModel = new ProfileModel();
    }

    initializePage() {
        this.activeRoute.params.subscribe((param: any) => {
            const val = atob(param['id']);
            this.profileModel.id = val;
            this.getProfile(this.profileModel.id);
        });
    }

    getProfile(id:string){      
        this.userService.get(id).subscribe(response=>{
            const result = response.json();
            if(result.code === '00'){
                this.profileModel = result.data;
                this.profileModel.eventType = 'Edit';
            }
        }, error=>{

        }) 
    }
}