import { Component, OnInit, Input, ElementRef, ViewChild } from '@angular/core';
import { RoleService } from "../services/role.service";
import { ProfileModel } from "../../user/models/profile.model";
import { RoleModel } from "../models/role.model";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";
declare var jQuery: any;

@Component({
    selector: 'role-profile',
    templateUrl: '../views/role-profile.component.html'
})

export class RoleProfileComponent implements OnInit {

    @Input() profileModel: ProfileModel;
    roles = [];
    isLoading: boolean = false;
    role:RoleModel = null;
    @ViewChild('removeRole') removeRoleModal: ElementRef;
    constructor(private roleService: RoleService, private toasterService: ToasterService) {
        this.profileModel = new ProfileModel();
    }

    ngOnInit() {
        console.log(this.profileModel);
        if (this.profileModel.id.length > 0) {
            this.getCurrentUserDomain(this.profileModel.id);
        }
    }

    loadRemoveModal() {
        if(this.role === null){
            return;
        }
        jQuery(this.removeRoleModal.nativeElement).modal('show');
    }

    getCurrentUserDomain(id: string) {
        this.roleService.getUserRole(id).subscribe(response => {            
            const resp = Object.assign(new ResponseModel(),response.json());
            const roles = Object.assign([], resp.data);
            this.roles = roles;

        }, error => {
        })
    }

    roleAction(){
        this.isLoading = true;
        this.roleService.removeRole(this.profileModel.id,this.role.id)
        .subscribe(response=>{
            this.isLoading = false;
            console.log(response);
            const resp = Object.assign(new ResponseModel(),response.json());
            if(resp.code == '00'){
                this.getCurrentUserDomain(this.profileModel.id);
                jQuery(this.removeRoleModal.nativeElement).modal('hide');  
                this.toasterService.pop('success','Success',resp.description);              
            }
        }, error=>{
            this.isLoading = false;
            jQuery(this.removeRoleModal.nativeElement).modal('hide');
            this.toasterService.pop('error','Error',error);
        })        
    }
}