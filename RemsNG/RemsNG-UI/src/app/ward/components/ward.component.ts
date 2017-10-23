import { Component, ElementRef, ViewChild, OnInit } from '@angular/core';
import { PageModel } from '../../shared/models/page.model';
import { WardModel } from '../models/ward.model';
import { AppSettings } from '../../shared/models/app.settings';
import { ToasterService } from 'angular2-toaster';
import { LcdaService } from '../../lcda/services/lcda.services';
import { WardService } from '../services/ward.service';
import { ResponseModel } from '../../shared/models/response.model';
import { RouterModule, Routes, ActivatedRoute } from '@angular/router';
import { LcdaModel } from "../../lcda/models/lcda.models";
declare var jQuery: any;

@Component({
    selector: 'app-ward',
    templateUrl: '../views/ward.component.html'
})
export class WardComponent implements OnInit {
    wardlst = [];
    pageModel: PageModel;
    wardModel: WardModel;
    lgda: LcdaModel;
    isLoading: boolean = false;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changestatus') changestatusModal: ElementRef;

    constructor(private activeRoute: ActivatedRoute, private appSettings: AppSettings,
        private toasterService: ToasterService, private lcdaService: LcdaService
        , private wardService: WardService) {

        this.pageModel = new PageModel();
        this.wardModel = new WardModel();
        this.lgda = new LcdaModel();
    }

    open(eventType: string, data: any) {
        if (eventType === this.appSettings.editMode) {
            this.wardModel = data;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.addMode) {
            this.wardModel = new WardModel();
            this.wardModel.lcdaId = this.lgda.id;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.changeStatusMode) {
            this.wardModel = data;
            jQuery(this.changestatusModal.nativeElement).modal('show');
        }
        this.wardModel.eventType = eventType;
    }

    ngOnInit() {
        this.activeRoute.params.subscribe((param: any) => {
            this.getLcda(param["id"]);
        });
    }

    getLcda(id: string) {
        this.lcdaService.getLCdaById(id).subscribe(response => {

            const result = Object.assign(new ResponseModel(), response.json());
            if (result.code == '00') {
                this.lgda = Object.assign(new LcdaModel(), result.data);
                this.getWard();
            }
        }, error => {

        })
    }

    getWard() {
        this.isLoading = true;

        this.wardService.getWard(this.pageModel,this.lgda.id).subscribe(response => {
            const result = response.json();
            const resultScheme = { data: [], totalPageCount: 0 };
            const responseD = Object.assign(resultScheme, result);
            if (responseD.data.length > 0) {
                this.wardlst = responseD.data;
                this.pageModel.totalPageCount = responseD.totalPageCount;
            } else {
                this.pageModel.pageNum = this.pageModel.pageNum > 1 ? this.pageModel.pageNum -= 1 : this.pageModel.pageNum;
            }
            this.isLoading = false;
        }, error => {
            this.isLoading = false;
        });
    }

    addWard() {
        if (this.wardModel.wardName.length < 1) {
            this.alertMsg(this.appSettings.danger, 'Ward Name is required!!');
            return;
        } else if (this.wardModel.lcdaId.length < 1) {
            this.alertMsg(this.appSettings.danger, 'LGDA is required!!');
            return;
        }

        this.wardModel.isLoading = true;
        if (this.wardModel.eventType === this.appSettings.addMode) {
            this.wardService.addWard(this.wardModel).subscribe(response => {
                this.wardModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getWard();
                }
            }, error => {
                this.wardModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.wardModel.eventType === this.appSettings.addMode || this.wardModel.eventType === this.appSettings.editMode) {
                    jQuery(this.addModal.nativeElement).modal('hide');
                } else if (this.wardModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            });
        } else if (this.wardModel.eventType === this.appSettings.editMode) {
            this.wardModel.isLoading = false;
            this.wardService.editWard(this.wardModel).subscribe(response => {
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getWard();
                }
            }, error => {
                this.wardModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.wardModel.eventType === this.appSettings.addMode || this.wardModel.eventType === this.appSettings.editMode) {
                    jQuery(this.addModal.nativeElement).modal('hide');
                } else if (this.wardModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            });
        } else if (this.wardModel.eventType === this.appSettings.changeStatusMode) {
            this.wardModel.wardStatus = this.wardModel.wardStatus === 'ACTIVE' ? 'NOT_ACTIVE' : 'ACTIVE';
            this.wardService.changeStatusWard(this.wardModel).subscribe(response => {
                this.wardModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                    this.getWard();
                } else {
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.wardModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                if (this.wardModel.eventType === this.appSettings.addMode || this.wardModel.eventType === this.appSettings.editMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                } else if (this.wardModel.eventType === this.appSettings.changeStatusMode) {
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            });
        }
    }

    alertMsg(ngclass: string, msg: string) {
        this.wardModel.errClass = new Array(ngclass);
        this.wardModel.errMsg = msg;
        this.wardModel.isErrMsg = true;
        setTimeout(() => {
            this.wardModel.errClass.pop();
            this.wardModel.errMsg = '';
            this.wardModel.isErrMsg = false;
        }, 3000);
    }

}

