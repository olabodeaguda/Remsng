import { Component, OnInit, ViewEncapsulation, Input, ViewChild, ElementRef } from '@angular/core';
import { DomainModel } from '../models/domain.model';
import { DomainService } from '../services/domain.service';
import { PageModel } from '../../shared/models/page.model';
import { Validators, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { AppSettings } from '../../shared/models/app.settings';
import { ResponseModel } from '../../shared/models/response.model';
import { StateService } from '../../shared/services/state.service';
declare var jQuery: any;

@Component({
    selector: 'app-domain',
    templateUrl: '../views/domain.component.html'
})

export class DomainComponent implements OnInit {
    domainLst = [];
    stateLst = [];
    pageModel: PageModel;
    isLoading: boolean = false;
    domainModel: DomainModel;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changestatus') changestatusModal: ElementRef;
    constructor(private domainService: DomainService,
        private appSettings: AppSettings,
        private stateSrv:StateService) {
        this.pageModel = new PageModel();
        this.domainModel = new DomainModel();
    }

    ngOnInit() {
        this.getDomain();
        this.getStates();
    }

    getStates(){
        this.stateSrv.GetStates().subscribe(
            response=>{
                this.stateLst = response;
            },
            error=>{

            }
        );
    }

    open(eventType: string, data: any) {
        if (eventType === this.appSettings.editMode) {
            this.domainModel = data;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.addMode) {
            this.domainModel = new DomainModel();
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.changeStatusMode) {
            this.domainModel = data;
            jQuery(this.changestatusModal.nativeElement).modal('show');
        }
        this.domainModel.eventType = eventType;
    }

    addDomain() {
        this.domainModel.isLoading = true;
        if (this.domainModel.domainCode.trim().length < 1) {
            this.alertMsg(this.appSettings.warning, 'Domain Code is required!!!');
            return;
        } else if (this.domainModel.domainName.trim().length < 1) {
            this.alertMsg(this.appSettings.warning, 'Domain Name is required!!!');
            return;
        }

        if (this.domainModel.eventType === this.appSettings.addMode) {
            this.domainService.add(this.domainModel).subscribe(response => {
                this.domainModel.isLoading = false;
                const resp = Object.assign(new ResponseModel(), response);
                if (resp.code === '00') {
                    this.domainModel = new DomainModel();
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getDomain();
                } else {
                    this.alertMsg(this.appSettings.danger, resp.description || 'An error occur, please try again or contact administrator');
                }
            }, error => {
                this.domainModel.isLoading = false;
                this.alertMsg(this.appSettings.danger, error || 'An error occur, please try again or contact administrator');
            });
        } else if (this.domainModel.eventType === this.appSettings.editMode) {
            this.domainService.edit(this.domainModel).subscribe(response => {
                this.domainModel.isLoading = false;
                const resp = Object.assign(new ResponseModel(), response);
                if (resp.code === '00') {
                    this.domainModel = new DomainModel();
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getDomain();
                } else {
                    this.alertMsg(this.appSettings.danger, resp.description || 'An error occur, please try again or contact administrator');
                }
            }, error => {
                this.getDomain();
                this.domainModel.isLoading = false;
                this.alertMsg(this.appSettings.danger, error || 'An error occur, please try again or contact administrator');
            });
        } else if (this.domainModel.eventType === this.appSettings.changeStatusMode) {
            this.domainModel.domainStatus = this.domainModel.domainStatus === 'ACTIVE' ? 'NOT_ACTIVE' : 'ACTIVE';
            this.domainService.changeStatus(this.domainModel).subscribe(response => {
                this.domainModel.isLoading = false;
                const resp = Object.assign(new ResponseModel(), response);
                if (resp.code === '00') {
                    this.domainModel = new DomainModel();
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                    this.getDomain();
                } else {
                    this.alertMsg(this.appSettings.danger, resp.description || 'An error occur, please try again or contact administrator');
                }
            }, error => {
                this.getDomain();
                this.domainModel.isLoading = false;
                this.alertMsg(this.appSettings.danger, error || 'An error occur, please try again or contact administrator');
            });
        }
    }

    alertMsg(ngclass: string, msg: string) {
        this.domainModel.errClass = new Array(ngclass);
        this.domainModel.msg = msg;
        this.domainModel.isErrMsg = true;
        setTimeout(() => {
            this.domainModel.errClass.pop();
            this.domainModel.msg = '';
            this.domainModel.isErrMsg = false;
        }, 3000);
    }

    getDomain() {
        this.isLoading = true;
        this.domainService.all(this.pageModel).subscribe(response => {
            this.isLoading = false;
            const result = response;
            const resultScheme = { data: [], totalPageCount: 0 };
            const responseD = Object.assign(resultScheme, result);
            if (responseD.data.length > 0) {
                this.domainLst = responseD.data;
                this.pageModel.totalPageCount = responseD.totalPageCount;
            } else {
                this.pageModel.pageNum = this.pageModel.pageNum > 1 ? this.pageModel.pageNum -= 1 : this.pageModel.pageNum;
            }
        },
            error => {
                this.isLoading = false;
            });
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.domainLst.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getDomain();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getDomain();
    }
}





























































































































































































































