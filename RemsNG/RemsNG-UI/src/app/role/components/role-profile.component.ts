import { Component, OnInit, Input, ElementRef, ViewChild } from '@angular/core';
import { RoleService } from "../services/role.service";
import { ProfileModel } from "../../user/models/profile.model";
import { RoleModel } from "../models/role.model";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { LcdaService } from "../../lcda/services/lcda.services";
import { AssignDomainModel } from "../../user/models/assign-domain.model";
import { AppSettings } from "../../shared/models/app.settings";
import { LcdaModel } from "../../lcda/models/lcda.models";
import { AssignRoleModel } from "../../user/models/assig-role.model";
declare var jQuery: any;

@Component({
    selector: 'role-profile',
    templateUrl: '../views/role-profile.component.html'
})

export class RoleProfileComponent implements OnInit {

    @Input() profileModel: ProfileModel;
    selectRole: RoleModel;
    selectedDomain: LcdaModel;
    lgdas = [];
    isLoading: boolean = false;
    role: RoleModel;
    roles = [];
    assigndomainmodel: AssignDomainModel;
    unassignDomainArray: LcdaModel[] = []
    assignRoleModel: AssignRoleModel;
    @ViewChild('removeRole') removeRoleModal: ElementRef;
    @ViewChild('removeDomain') removeDomainModal: ElementRef;
    @ViewChild('assignlgda') assignlgdaModal: ElementRef;
    @ViewChild('assignrolemodal') assignrolemodal: ElementRef;

    constructor(private toasterService: ToasterService,
        private lcdaservice: LcdaService,
        private roleservice: RoleService,
        private appSettings: AppSettings) {
        this.profileModel = new ProfileModel();
        this.selectRole = new RoleModel();
        this.selectedDomain = new LcdaModel();
        this.selectRole.roleName = 'Nil';
        this.assigndomainmodel = new AssignDomainModel();
        this.assignRoleModel = new AssignRoleModel();
        this.role = new RoleModel();
    }

    ngOnInit() {
        if (this.profileModel.id.length > 0) {
            this.getCurrentUserDomain();
        }
    }

    loadRemoveModal(eventType: string) {
        if (eventType === 'REMOVE_ROLE') {
            if (this.selectRole.id.length < 1) {
                return;
            }
            jQuery(this.removeRoleModal.nativeElement).modal('show');
        } else if (eventType === 'REMOVE_DOMAIN') {
            if (this.selectedDomain.id.length < 1) {
                return;
            }
            jQuery(this.removeDomainModal.nativeElement).modal('show');
        }
    }

    getCurrentUserDomain() {
        this.lcdaservice.getLcdaByuserId(this.profileModel.id).subscribe(response => {
            const resp = Object.assign(new ResponseModel(), response);
            this.lgdas = Object.assign([], resp);
        }, error => {
            this.toasterService.pop('error', 'Error', error);
        })
    }

    roleAction(eventType) {
        this.role.isLoading = true;
        if (eventType === 'REMOVE_ROLE') {
            this.roleservice.removeRole(this.profileModel.id, this.selectRole.id)
                .subscribe(response => {
                    this.role.isLoading = false;
                    const resp = Object.assign(new ResponseModel(), response);
                    if (resp.code == '00') {
                        this.getCurrentUserDomain();
                        this.selectRole = new RoleModel();
                        jQuery(this.removeRoleModal.nativeElement).modal('hide');
                        this.toasterService.pop('success', 'Success', resp.description);
                    }
                }, error => {
                    this.role.isLoading = false;
                    jQuery(this.removeRoleModal.nativeElement).modal('hide');
                    this.toasterService.pop('error', 'Error', error);
                })
        } else if (eventType === 'REMOVE_DOMAIN') {
            this.selectedDomain.isLoading = true;
            this.lcdaservice.removeUserFromLCDA(this.selectedDomain.id, this.profileModel.id)
                .subscribe(responnse => {
                    this.selectedDomain.isLoading = false;
                    const result = Object.assign(new ResponseModel(), responnse);
                    if (result.code == '00') {
                        this.getCurrentUserDomain();
                        this.toasterService.pop('success', 'Success', result.description);
                        jQuery(this.removeDomainModal.nativeElement).modal('hide');
                    }
                    else {
                        this.toasterService.pop('error', 'Error', result.description);
                        jQuery(this.removeDomainModal.nativeElement).modal('hide');
                    }
                }, error => {
                    this.selectedDomain.isLoading = false;
                    this.toasterService.pop('error', 'Error', error);
                    jQuery(this.removeDomainModal.nativeElement).modal('hide');
                })
        }
    }

    open(eventType: string) {
        if (eventType === this.appSettings.assignLGDA) {
            this.getUnAssignedDomain();
            this.assignRoleModel = new AssignRoleModel();
        }
        else if (eventType === this.appSettings.assignRole) {
            if (this.lgdas.length < 1) {
                this.toasterService.pop('error', 'Error', 'Zero roles have been assign to this user');
                return;
            }
            else if(this.lgdas.length === 1){
                const dId = this.lgdas[0].id;
                this.GetRoleByDomainId(dId);
            }
            this.assignRoleModel = new AssignRoleModel();
            jQuery(this.assignrolemodal.nativeElement).modal('show');
        }
    }

    GetRoleByDomainId(domainId: string){
        this.roleservice.roleByDomainId(domainId).subscribe(response => {
            this.isLoading = false;
            this.roles = Object.assign([], response);
            this.assignRoleModel.roleId = null;
        }, error => {
            this.isLoading = false;
        });
    }

    domainSelectedChange(element: any, fromType: string) {
        this.isLoading = true;
        if (fromType === 'ALL_DOMAINROLE') {
           this.GetRoleByDomainId(element);
        } else if (fromType === 'ALLUSER_DOMAINROLE') {
            this.selectedDomain = element;
            // get role assign to users in this domain
            this.roleservice.roleByDomainIdUserId(element.id, this.profileModel.id).subscribe(
                response => {
                    this.isLoading = false;
                    const schema = { code: '', data: new RoleModel() };
                    const resp = Object.assign(schema, response);
                    if (resp.code === '00') {                        
                        if(resp.data !== null){
                            this.selectRole = resp.data;         
                        } else{
                            this.selectRole = new RoleModel();
                        }
                    }
                },
                error => {
                    this.isLoading = false;
                })
        }
    }

    getUnAssignedDomain() {
        this.isLoading = true;
        this.lcdaservice.unAssignedDomainToUserbyUserId(this.profileModel.id).subscribe(response => {
            this.isLoading = false;
            const result = Object.assign(new ResponseModel(), response);
            if (result.code === "00") {
                this.unassignDomainArray = result.data;
                jQuery(this.assignlgdaModal.nativeElement).modal('show');
            }
        }, error => {
            this.isLoading = false;
        })
    }

    action(eventType: string) {
        if (eventType === this.appSettings.assignLGDA) {
            if (this.assigndomainmodel.lgdaId.length < 0) {
                this.toasterService.pop('error', 'Error', '');
                return;
            }
            this.assigndomainmodel.userId = this.profileModel.id;
            this.lcdaservice.assignLGDAToUser(this.assigndomainmodel).subscribe(response => {
                this.profileModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.getCurrentUserDomain();
                    this.toasterService.pop('success', 'Success', result.description);
                    this.assigndomainmodel = new AssignDomainModel();
                    this.selectedDomain = new LcdaModel();
                    jQuery(this.assignlgdaModal.nativeElement).modal('hide');
                } else {
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.profileModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                jQuery(this.assignlgdaModal.nativeElement).modal('hide');
            });
        } else if (eventType === this.appSettings.assignRole) {
            this.assignRoleModel.isLoading = true;
            this.assignRoleModel.userId = this.profileModel.id;
            this.roleservice.assignRoleTouser(this.assignRoleModel).subscribe(
                response => {
                    this.assignRoleModel.isLoading = false;
                    const result = Object.assign(new ResponseModel(), response);
                    if (result.code === '00') {
                        this.toasterService.pop('success', 'Success', result.description);
                        this.assignRoleModel = new AssignRoleModel();
                        this.selectedDomain = new LcdaModel();

                        jQuery(this.assignrolemodal.nativeElement).modal('hide');
                    } else {
                        this.toasterService.pop('error', 'Error', result.description);
                    }
                },
                error => {
                    this.assignRoleModel.isLoading = false;
                    this.toasterService.pop("error", "Error", error);
                })
        }
    }
}