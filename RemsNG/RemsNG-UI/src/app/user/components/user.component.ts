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
declare var jQuery: any;

@Component({
    selector: 'app-user',
    templateUrl: '../views/user.component.html'
})

export class UserComponent implements OnInit {
    userLst = []
    profileModel: ProfileModel;
    assigndomainmodel: AssignDomainModel;
    pageModel: PageModel;
    changePwd: ChangePasswordModel;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changestatus') changestatusModal: ElementRef;
    @ViewChild('changepwd') changePwdModal: ElementRef;
    @ViewChild('assignlgda') assignlgdaModal: ElementRef;
    isLoading: boolean = false;
    lcdaLst = []

    constructor(private userService: UserService, private appSettings: AppSettings,
        private toasterService: ToasterService, private lcdaService: LcdaService) {
        this.profileModel = new ProfileModel();
        this.pageModel = new PageModel();
        this.changePwd = new ChangePasswordModel();
        this.assigndomainmodel = new AssignDomainModel();
    }

    ngOnInit() {
        this.getProfile();
        this.getLcda();
    }

    getLcda() {
        this.lcdaService.all().subscribe(response => {
            this.lcdaLst = response.json();
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
        }
        this.profileModel.eventType = eventType;
    }

    getProfile() {
        this.isLoading = true;
        this.userService.getProfile(this.pageModel)
            .subscribe(response => {
                const result = response.json();
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
                this.isLoading = true;
            });
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
                const result = Object.assign(new ResponseModel(), response.json());
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
                const result = Object.assign(new ResponseModel(), response.json());
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
                const result = Object.assign(new ResponseModel(), response.json());
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
                const result = Object.assign(new ResponseModel(), response.json());
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
                const result = Object.assign(new ResponseModel(), response.json());
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