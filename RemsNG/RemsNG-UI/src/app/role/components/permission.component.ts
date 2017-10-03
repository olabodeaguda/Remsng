import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PermissionService } from '../services/permission.service';
import { RoleService } from "../services/role.service";
import { RoleModel } from "../models/role.model";
import { PageModel } from "../../shared/models/page.model";
import { AppSettings } from "../../shared/models/app.settings";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { PermissionModel } from "../models/permission.model";
import { UserPermissionModel } from "../models/user-permission.model";
declare var jQuery: any;

@Component({
    selector: 'app-permission',
    templateUrl: '../views/permission.component.html'
})

export class PermissionComponent implements OnInit {

    userPerm: UserPermissionModel;
    roleModel: RoleModel;
    permissionDistinct = [];
    permissionLst = [];
    isLoading: boolean = false;
    pageModel: PageModel;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('removeModal') removeModal: ElementRef;

    constructor(private activeRoute: ActivatedRoute,
        private permissionService: PermissionService,
        private roleService: RoleService,  private appSettings: AppSettings,
        private toasterService: ToasterService) {
            this.pageModel = new PageModel();
            this.roleModel = new RoleModel();
            this.userPerm = new UserPermissionModel();
    }

    ngOnInit() {
        this.initializePage();
    }

    open(eventType: string, data: any) {
        if (eventType === this.appSettings.addMode) {
            this.roleModel = new RoleModel();
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.removeMode) {
            this.roleModel = data;
            jQuery(this.removeModal.nativeElement).modal('show');
        }
        this.roleModel.eventType = eventType;
    }

    initializePage() {
        this.activeRoute.params.subscribe((param: any) => {
            this.getRole(param["id"]);                        
        });
    }

    getPermissionByRoleId() {
        this.isLoading = true;
        this.permissionService.getPermissionByROleId(this.roleModel.id, this.pageModel).subscribe(response => {
            this.isLoading = false;
            this.permissionLst = response.json();
        }, error => {
            this.isLoading = false;
        })
    }

    getPermissionNotInRole() {
        this.isLoading = true;
        this.permissionService.getPermissionNotInRole(this.roleModel.id).subscribe(response => {       
            this.isLoading = false;     
            this.permissionDistinct = response.json();
        },error =>{
            this.isLoading = false;
        });
        
    }

    getRole(id: string) {
        this.isLoading = true;
        this.roleService.get(id).subscribe(response =>{
            this.isLoading = false;
            this.roleModel = Object.assign(new RoleModel(), response.json());            
            this.getPermissionByRoleId();
            this.getPermissionNotInRole();
        }, error => {
            this.isLoading = false;
        })
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.permissionLst.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getPermissionByRoleId();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getPermissionByRoleId();
    }

}


