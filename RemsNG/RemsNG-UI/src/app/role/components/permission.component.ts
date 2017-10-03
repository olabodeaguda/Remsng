import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PermissionService } from '../services/permission.service';
import { RoleService } from "../services/role.service";
import { RoleModel } from "../models/role.model";
import { PageModel } from "../../shared/models/page.model";
import { AppSettings } from "../../shared/models/app.settings";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { PermissionModel } from "../models/permission.model";
import { RolePermissionModel } from "../models/role-permission.model";
import { ResponseModel } from "../../shared/models/response.model";
declare var jQuery: any;

@Component({
    selector: 'app-permission',
    templateUrl: '../views/permission.component.html'
})

export class PermissionComponent implements OnInit {

    userPerm: RolePermissionModel;
    roleModel: RoleModel;
    permissionDistinct = [];
    permissionLst = [];
    isLoading: boolean = false;
    pageModel: PageModel;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('removeModal') removeModal: ElementRef;

    constructor(private activeRoute: ActivatedRoute,
        private permissionService: PermissionService,
        private roleService: RoleService, private appSettings: AppSettings,
        private toasterService: ToasterService) {
        this.pageModel = new PageModel();
        this.roleModel = new RoleModel();
        this.userPerm = new RolePermissionModel();
    }

    ngOnInit() {
        this.initializePage();
    }

    open(eventType: string, data: any) {
        if (eventType === this.appSettings.addMode) {
            this.userPerm = new RolePermissionModel();
            this.userPerm.roleId = this.roleModel.id;
            this.userPerm.eventType = this.appSettings.addMode;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.removeMode) {
            this.userPerm.permissionId = data.id;
            this.userPerm.roleId = this.roleModel.id;
            this.userPerm.permissionName = data.permissionName;
            this.userPerm.eventType = this.appSettings.removeMode;
            jQuery(this.removeModal.nativeElement).modal('show');
        }
        this.userPerm.eventType = eventType;
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
        }, error => {
            this.isLoading = false;
        });

    }

    getRole(id: string) {
        this.isLoading = true;
        this.roleService.get(id).subscribe(response => {
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

    permissionActions() {

        if (this.userPerm.permissionId == undefined) {
            this.toasterService.pop('error', 'Error', 'Permission is required');
            return;
        } else if (this.userPerm.roleId == undefined) {
            return;
        }

        this.userPerm.isLoading = true;
        if (this.userPerm.eventType === this.appSettings.addMode) {
            this.roleService.assignPermissionToRole(this.userPerm).subscribe(response => {
                this.userPerm.isLoading = false;
                this.notifyUI(response);
                jQuery(this.addModal.nativeElement).modal('hide');
            }, error => {
                this.userPerm.isLoading = false;
            });
        } else if (this.userPerm.eventType === this.appSettings.removeMode) {
            this.roleService.removePermission(this.userPerm).subscribe(response => {
                this.userPerm.isLoading = false;
                this.notifyUI(response);
                jQuery(this.removeModal.nativeElement).modal('hide');
            }, error => {
                this.userPerm.isLoading = false;
            });
        }
    }

    notifyUI(response: Response) {
        const result = Object.assign(new ResponseModel(), response.json());
        if (result.code === "00") {
            this.toasterService.pop('success', 'Success', result.description);
            this.getPermissionByRoleId();
            this.getPermissionNotInRole();
        } else {
            this.toasterService.pop('error', 'error', result.description);
        }
    }

}


