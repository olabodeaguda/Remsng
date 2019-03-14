import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { DemandNoticeSearch } from "../models/demand-notice.search";
import { AppSettings } from "../../shared/models/app.settings";
import { PageModel } from "../../shared/models/page.model";
import { DemandNoticeService } from "../services/demand-notice.service";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { WardService } from "../../ward/services/ward.service";
import { StreetService } from "../../street/services/street.service";
import { IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { DownloadRequestModel } from '../models/download-request.model';
import { DemandNoticeTaxpayerService } from '../services/demand-noticeTaxpayer.service';
declare var jQuery: any;
import * as FileSaver from 'file-saver'
import { Router } from '@angular/router';

@Component({
    selector: 'demand-notice',
    templateUrl: '../views/demand-notice-batch.component.html'
})

export class DemandNoticeBatchComponent implements OnInit {

    searchModel: DemandNoticeSearch;
    wardLst = [];
    streetLst = [];
    yrLst = [];
    demandNoticeLst = [];
    pageModel: PageModel;
    isLoading: boolean = false;
    downloadRequestmodel: DownloadRequestModel;
    batchNo: string;//
    @ViewChild('downRequestPrompt') downRequestPromptModal: ElementRef;
    @ViewChild('downloadRequestModal') downloadRequestModal: ElementRef;
    @ViewChild('PromptConstraint') PromptConstraint: ElementRef;
    isLoadingMini: boolean = false;
    dowloadRequestList = [];

    constructor(private appsettings: AppSettings,
        private demandnoticeservice: DemandNoticeService,
        private toasterService: ToasterService,
        private wardservice: WardService,
        private streetservice: StreetService,
        private dtsService: DemandNoticeTaxpayerService, private router: Router) {
        this.searchModel = new DemandNoticeSearch();
        this.pageModel = new PageModel();
        this.downloadRequestmodel = new DownloadRequestModel();
    }

    ngOnInit(): void {
        this.yrLst = this.appsettings.getYearList();
        this.getDemandNotice();
        this.getWards();
    }
    openConstraint() {
        jQuery(this.PromptConstraint.nativeElement).modal('show');
    }

    downloadDN(url: string) {
        this.isLoadingMini = true;
        this.demandnoticeservice.downloadRpt(url).map(response => {
            this.isLoadingMini = false;
            let blob = response;
            FileSaver.saveAs(blob, url + '.zip');
        }, error => {
            this.isLoadingMini = false;
            this.toasterService.pop('error', 'Download Error', error);
        }).subscribe();
    }

    searchDemandNotice() {
        if (this.searchModel.dateYear <= 0) {
            this.toasterService.pop('error', 'Error', 'Billing year is required')
            return;
        }

        this.isLoading = true;
        this.demandnoticeservice.searchDemandNotice1(this.searchModel, this.pageModel).subscribe(
            response => {
                const objschema = { data: [], totalPageCount: 0 };
                const res = Object.assign(objschema, response);
                this.demandNoticeLst = res.data;
                this.pageModel.totalPageCount = res.totalPageCount;
                this.isLoading = false;
            },
            error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });

    }

    submitDemandRequest() {
        if (this.searchModel.dateYear <= 0) {
            this.toasterService.pop('error', 'Error', 'Billing year is required')
            return;
        }
        if (this.searchModel.runArrearsCategory > 0) {
            this.searchModel.runArrears = true;
        }

        this.searchModel.isProcessingRequest = true;
        this.demandnoticeservice.add(this.searchModel).subscribe(
            response => {
                jQuery(this.PromptConstraint.nativeElement).modal('hide');
                this.searchModel.isProcessingRequest = false;
                if (response.code === '00') {
                    this.toasterService.pop('success', 'Success', response.description);
                    this.getDemandNotice();
                    this.searchModel = new DemandNoticeSearch();
                } else {
                    this.toasterService.pop('error', 'Error', response.description);
                }
            },
            error => {
                jQuery(this.PromptConstraint.nativeElement).modal('hide');
                this.searchModel.isProcessingRequest = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    loadStreets(event) {
        this.getStreet(this.searchModel.wardId);
    }

    getWards() {
        this.isLoading = true;
        this.wardservice.all().subscribe(response => {
            this.wardLst = response;
            this.isLoading = false;
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        });
    }

    open(target: string, data: any) {
        if (target === 'RAISE_REQUEST') {
            if (data.batchNo.length <= 0) {
                return;
            }
            this.batchNo = data.batchNo;
            jQuery(this.downRequestPromptModal.nativeElement).modal('show');
        } else if (target === 'DOWNLOAD_REQUEST') {
            if (data.batchNo.length <= 0) {
                return;
            }
            this.batchNo = data.batchNo;
            jQuery(this.downloadRequestModal.nativeElement).modal('show');

            this.getRaisedRequest(this.batchNo);
            // setInterval(() => {
            //     this.getRaisedRequest(this.batchNo);
            // }, 30000);
        }
    }

    getRaisedRequest(batchId: string) {
        this.isLoading = true;
        this.demandnoticeservice.getRaisedRequest(batchId)
            .subscribe(response => {
                this.isLoading = false;
                this.dowloadRequestList = response;
            }, error => {
                this.isLoading = false;
            })
    }

    addRaiseRequest() {
        if (this.batchNo.length <= 0) {
            return;
        }
        this.isLoadingMini = true;
        this.demandnoticeservice.adDownloadRequest(this.batchNo)
            .subscribe(response => {
                this.isLoadingMini = false;
                if (response.code === '00') {
                    this.toasterService.pop("success", "Sucess", response.description);
                    jQuery(this.downRequestPromptModal.nativeElement).modal('hide');
                } else {
                    this.toasterService.pop("error", "Error", response.description);
                }
            }, error => {
                this.isLoadingMini = false;
                this.toasterService.pop("error", "Error", error);
            })
    }

    getStreet(wardId: string) {
        this.isLoading = true;
        this.streetservice.byWardId(wardId).subscribe(response => {
            this.isLoading = false;
            this.streetLst = response;
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    getDemandNotice() {
        this.isLoading = true;
        this.demandnoticeservice.get(this.pageModel).subscribe(response => {
            const objschema = { data: [], totalPageCount: 0 };
            const res = Object.assign(objschema, response);
            this.demandNoticeLst = res.data;
            this.pageModel.totalPageCount = res.totalPageCount;
            this.isLoading = false;
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    getDemandNotice2() {
        this.demandnoticeservice.get2(this.pageModel).subscribe(response => {
            const objschema = { data: [], totalPageCount: 0 };
            const res = Object.assign(objschema, response);
            this.demandNoticeLst = res.data;
            this.pageModel.totalPageCount = res.totalPageCount;
        }, error => {
            //  this.toasterService.pop('error', 'Error', error);
        })
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.demandNoticeLst.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getDemandNotice();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getDemandNotice();
    }

}