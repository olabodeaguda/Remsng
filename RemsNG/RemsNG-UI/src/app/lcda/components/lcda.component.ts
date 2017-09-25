import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { PageModel } from '../../shared/models/page.model';
import { LcdaModel } from '../models/lcda.models';
import { LcdaService } from '../services/lcda.services';
import { AppSettings } from '../../shared/models/app.settings';
import { ToasterService } from 'angular2-toaster';
import { DomainModel } from '../../domain/models/domain.model';
import { DomainService } from '../../domain/services/domain.service';
import { ResponseModel } from '../../shared/models/response.model';
declare var jQuery: any;

@Component({
    selector: 'app-lcda',
    templateUrl: '../views/lcda.component.html'
})

export class LcdaComponent implements OnInit {

    lcdaLst = [];
    pageModel: PageModel;
    lcdaModel: LcdaModel;
    domainList;
    isLoading: boolean = false;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changestatus') changestatusModal: ElementRef;

    constructor(private lcdaService: LcdaService, private appSettings: AppSettings,
        private toasterService: ToasterService, private domainService: DomainService) {
        this.pageModel = new PageModel();
        this.lcdaModel = new LcdaModel();
    }

    ngOnInit() {
        this.getLcda();
        this.domainService.activeDomains().subscribe(response => {
            this.domainList = response.json();
        }, error => {
        });
    }

    getLcda() {
        this.isLoading = true;

        this.lcdaService.getLcda(this.pageModel).subscribe(response => {
            const result = response.json();
            const resultScheme = { data: [], totalPageCount: 0 };
            const responseD = Object.assign(resultScheme, result);
            if (responseD.data.length > 0) {
                this.lcdaLst = responseD.data;
                this.pageModel.totalPageCount = responseD.totalPageCount;
            } else {
                this.pageModel.pageNum = this.pageModel.pageNum > 1 ? this.pageModel.pageNum -= 1 : this.pageModel.pageNum;
            }
            this.isLoading = false;
        }, error => {
            this.isLoading = false;
        });
    }

    open(eventType: string, data: any) {
        if (eventType === this.appSettings.editMode) {
            this.lcdaModel = data;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.addMode) {
            this.lcdaModel = new LcdaModel();
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.changeStatusMode) {
            this.lcdaModel = data;
            jQuery(this.changestatusModal.nativeElement).modal('show');
        }
        this.lcdaModel.eventType = eventType;
    }

    addLCDA() {
        if (this.lcdaModel.domainId === '') {
            this.alertMsg(this.appSettings.danger, 'Domain is required!!!');
            return;
        } else if (this.lcdaModel.lcdaName === '') {
            this.alertMsg(this.appSettings.danger, 'LCDA name is required!!!');
            return;
        } else if (this.lcdaModel.lcdaCode === '') {
            this.alertMsg(this.appSettings.danger, 'LCDA code is required!!!');
            return;
        }

        this.lcdaModel.isLoading = true;
        if (this.lcdaModel.eventType === this.appSettings.addMode) {
            this.lcdaService.addLCDA(this.lcdaModel).subscribe(response => {
                this.lcdaModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getLcda();
                }
            }, error => {
                this.lcdaModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.lcdaModel.eventType === this.appSettings.addMode || this.lcdaModel.eventType === this.appSettings.editMode) {
                    jQuery(this.addModal.nativeElement).modal('hide');
                } else if (this.lcdaModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            });
        } else if (this.lcdaModel.eventType === this.appSettings.editMode) {
            this.lcdaModel.isLoading = false;
            this.lcdaService.editLCDA(this.lcdaModel).subscribe(response => {
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getLcda();
                }
            }, error => {
                this.lcdaModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.lcdaModel.eventType === this.appSettings.addMode || this.lcdaModel.eventType === this.appSettings.editMode) {
                    jQuery(this.addModal.nativeElement).modal('hide');
                } else if (this.lcdaModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            });
        } else if (this.lcdaModel.eventType === this.appSettings.changeStatusMode) {
            this.lcdaModel.lcdaStatus = this.lcdaModel.lcdaStatus === 'ACTIVE' ? 'NOT_ACTIVE' : 'ACTIVE';
            this.lcdaService.changeStatusLCDA(this.lcdaModel).subscribe(response => {
                this.lcdaModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                    this.getLcda();
                } else {
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.lcdaModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.lcdaModel.eventType === this.appSettings.addMode || this.lcdaModel.eventType === this.appSettings.editMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                } else if (this.lcdaModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            });
        }
    }

    alertMsg(ngclass: string, msg: string) {
        this.lcdaModel.errClass = new Array(ngclass);
        this.lcdaModel.errMsg = msg;
        this.lcdaModel.isErrMsg = true;
        setTimeout(() => {
            this.lcdaModel.errClass.pop();
            this.lcdaModel.errMsg = '';
            this.lcdaModel.isErrMsg = false;
        }, 3000);
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.lcdaLst.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getLcda();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getLcda();
    }
}
