import { Component, ViewChild, ElementRef, OnInit } from '@angular/core';
import { ProfileModel } from '../models/profile.model';
import { PageModel } from '../../shared/models/page.model';
import { UserService } from '../services/user.service';
import { AppSettings } from '../../shared/models/app.settings';
import { ResponseModel } from '../../shared/models/response.model';
import { ToasterService } from 'angular2-toaster';
import { ChangePasswordModel } from '../models/change-password.model';
import { AssignDomainModel } from '../models/assign-domain.model';
import { LcdaService } from '../../lcda/services/lcda.services';
import { RoleService } from '../../role/services/role.service';
import { RoleModel } from '../../role/models/role.model';
import { AssignRoleModel } from '../models/assig-role.model';
import { Router } from "@angular/router";
declare var jQuery: any;

@Component({
    selector: 'app-user',
    templateUrl: '../views/user.component.html'
})

export class UserComponent implements OnInit {
    userLst = []
    profileModel: ProfileModel;
    assignRoleModel: AssignRoleModel;
    assigndomainmodel: AssignDomainModel;
    pageModel: PageModel;
    changePwd: ChangePasswordModel;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changestatus') changestatusModal: ElementRef;
    @ViewChild('changepwd') changePwdModal: ElementRef;//
    @ViewChild('assignlgda') assignlgdaModal: ElementRef;
    @ViewChild('assignrole') assignroleModal: ElementRef;
    isLoading: boolean = false;
    lcdaLst = [];
    roles = [];
    roleModel: RoleModel;

    constructor(private userService: UserService,
        private appSettings: AppSettings,
        private toasterService: ToasterService,
        private lcdaService: LcdaService, private roleservice: RoleService,
        private router: Router) {
        this.profileModel = new ProfileModel();
        this.pageModel = new PageModel();
        this.changePwd = new ChangePasswordModel();
        this.assigndomainmodel = new AssignDomainModel();
        this.assignRoleModel = new AssignRoleModel();
        this.roleModel = new RoleModel();
    }

    ngOnInit() {
        this.getProfile();
        this.getLcda();
    }

    getLcda() {
        this.lcdaService.all().subscribe(response => {
            this.lcdaLst = response;
        }, error => { });
    }

    alertMsg(ngclass: string, msg: string) {
        this.profileModel.errClass = new Array(ngclass);
        this.profileModel.errMsg = msg;
        this.profileModel.isErrMsg = true;
        setTimeout(() => {
            this.profileModel.errClass.pop();
            this.profileModel.errMsg = '';
            this.profileModel.isErrMsg = false;
        }, 3000);
    }

    open(eventType: string, data: any) {
        if (eventType === this.appSettings.editMode) {
            this.profileModel = data;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.addMode) {
            this.profileModel = new ProfileModel();
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.changeStatusMode) {
            this.profileModel = data;
            jQuery(this.changestatusModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.changePwdMode) {
            this.profileModel = data;
            jQuery(this.changePwdModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.assignLGDA) {
            this.profileModel = data;
            jQuery(this.assignlgdaModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.assignRole) {
            this.profileModel = data;
            this.getCurrentUserRole(this.profileModel.id);
            this.getAllDomainRole(this.profileModel.username, true);
        } else if (eventType === this.appSettings.profileMode) {
            this.profileModel = data;
           // const val = btoa(this.profileModel.id);
            this.router.navigate(['user', this.profileModel.id])
        }
        this.profileModel.eventType = eventType;
    }

    getCurrentUserRole(id: string) {
        this.roleservice.getUserRole(id).subscribe(response => {            
            const resp = Object.assign(new ResponseModel(),response.json());
            const roles = Object.assign([], resp.data);

            if (roles.length > 0) {
                this.assignRoleModel.currentRole = roles[0];
            } else{
                
                this.assignRoleModel.currentRole.roleName = 'Anonymous';
            }

        }, error => {
        })
    }

    getProfile() {
        this.isLoading = true;
        this.userService.getProfile(this.pageModel)
            .subscribe(response => {
                const result = response;
                const resultScheme = { data: [], totalPageCount: 0 };
                const responseD = Object.assign(resultScheme, result);
                if (responseD.data.length > 0) {
                    this.userLst = responseD.data;
                    this.pageModel.totalPageCount = responseD.totalPageCount;
                } else {
                    this.pageModel.pageNum = this.pageModel.pageNum > 1 ? this.pageModel.pageNum -= 1 : this.pageModel.pageNum;
                }
                this.isLoading = false;
            }, error => {
                this.isLoading = false;
            });
    }

    getAllDomainRole(username: string, loadview: boolean) {
        this.isLoading = true;
        this.roleservice.getAllDomainRoles(username).subscribe(response => {
            this.roles = Object.assign([], response);
            this.isLoading = false;
        },
            error => {
                this.isLoading = false;
            }, () => {
                if (loadview) {
                    jQuery(this.assignroleModal.nativeElement).modal('show');
                }
            })
    }

    addUser() {
        if (this.profileModel.username === '') {
            this.alertMsg(this.appSettings.danger, 'Username is required');
            return;
        } else if (this.profileModel.email === '') {
            this.alertMsg(this.appSettings.danger, 'Email is required');
            return;
        } else if (this.profileModel.surname === '') {
            this.alertMsg(this.appSettings.danger, 'Surname is required');
            return;
        } else if (this.profileModel.firstname === '') {
            this.alertMsg(this.appSettings.danger, 'First name is required');
            return;
        } else if (this.profileModel.lastname === '') {
            this.alertMsg(this.appSettings.danger, 'Last name is required');
            return;
        } else if (this.profileModel.gender === '') {
            this.alertMsg(this.appSettings.danger, 'Gender is required');
            return;
        }

        this.profileModel.isLoading = true;
        if (this.profileModel.eventType === this.appSettings.addMode) {
            this.userService.add(this.profileModel).subscribe(response => {
                this.profileModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getProfile();
                }
            }, error => {
                this.profileModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.profileModel.eventType === this.appSettings.addMode || this.profileModel.eventType === this.appSettings.editMode) {
                    jQuery(this.addModal.nativeElement).modal('hide');
                } else if (this.profileModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            });
        } else if (this.profileModel.eventType === this.appSettings.editMode) {
            this.profileModel.isLoading = false;
            this.userService.update(this.profileModel).subscribe(response => {
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getProfile();
                }
            }, error => {
                this.profileModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.profileModel.eventType === this.appSettings.addMode || this.profileModel.eventType === this.appSettings.editMode) {
                    jQuery(this.addModal.nativeElement).modal('hide');
                } else if (this.profileModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            });
        } else if (this.profileModel.eventType === this.appSettings.changeStatusMode) {
            this.profileModel.userStatus = this.profileModel.userStatus === 'ACTIVE' ? 'NOT_ACTIVE' : 'ACTIVE';
            this.userService.changeStatus(this.profileModel).subscribe(response => {
                this.profileModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                    this.getProfile();
                } else {
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.profileModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.profileModel.eventType === this.appSettings.addMode || this.profileModel.eventType === this.appSettings.editMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                } else if (this.profileModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            });
        } else if (this.profileModel.eventType === this.appSettings.changePwdMode) {
            this.userService.changePwd(this.profileModel, this.changePwd).subscribe(response => {
                this.profileModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    this.changePwd = new ChangePasswordModel();
                    jQuery(this.changePwdModal.nativeElement).modal('hide');
                } else {
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.profileModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.profileModel.eventType === this.appSettings.addMode || this.profileModel.eventType === this.appSettings.editMode) {
                    jQuery(this.changePwdModal.nativeElement).modal('hide');
                } else if (this.profileModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changePwdModal.nativeElement).modal('hide');
                }
            });
        } else if (this.profileModel.eventType === this.appSettings.assignLGDA) {
            this.assigndomainmodel.userId = this.profileModel.id;
            this.lcdaService.assignLGDAToUser(this.assigndomainmodel).subscribe(response => {
                this.profileModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    this.assigndomainmodel = new AssignDomainModel();
                    jQuery(this.assignlgdaModal.nativeElement).modal('hide');
                } else {
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.profileModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                jQuery(this.assignlgdaModal.nativeElement).modal('hide');
            });
        } else if (this.profileModel.eventType === this.appSettings.assignRole) {
            this.assignRoleModel.isLoading = true;
            this.assignRoleModel.userId = this.profileModel.id;
            this.roleservice.assignRoleTouser(this.assignRoleModel).subscribe(
                response => {
                    this.assignRoleModel.isLoading = false;
                    const result = Object.assign(new ResponseModel(), response);
                    if (result.code === '00') {
                        this.toasterService.pop('success', 'Success', result.description);
                        this.assignRoleModel = new AssignRoleModel();
                        jQuery(this.assignroleModal.nativeElement).modal('hide');
                    } else {
                        this.toasterService.pop('error', 'Error', result.description);
                    }
                },
                error => {
                    this.assignRoleModel.isLoading = false;
                })
        }
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.userLst.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getProfile();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getProfile();
    }
}