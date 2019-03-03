import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { DemandNoticeSearch } from "../models/demand-notice.search";
import { AppSettings } from "../../shared/models/app.settings";
import { PageModel } from "../../shared/models/page.model";
import { DemandNoticeService } from "../services/demand-notice.service";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { WardService } from "../../ward/services/ward.service";
import { StreetService } from "../../street/services/street.service";
import { DownloadRequestModel } from '../models/download-request.model';
import { DemandNoticeTaxpayerService } from '../services/demand-noticeTaxpayer.service';
declare var jQuery: any;
import * as FileSaver from 'file-saver'
import { Router } from '@angular/router';
import { isNullOrUndefined, isNull } from 'util';
import { DemandNoticeTaxpayer } from '../models/demand-taxpayer-model';

@Component({
    selector: 'demand-notice',
    templateUrl: '../views/demand-notice.component.html'
})

export class DemandNoticeComponent implements OnInit {

    masterCheck: boolean = false;
    taxpayerIds = []
    searchModel: DemandNoticeSearch;
    wardLst = [];
    streetLst = [];
    yrLst = [];
    demandNoticeLst = [];
    pageModel: PageModel;
    isLoading: boolean = false;
    downloadRequestmodel: DownloadRequestModel;
    actionSelected: string;
    @ViewChild('promptRequest') promptRequest: ElementRef;
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
        this.GetDemandNotice(new Date().getFullYear());
        this.getWards();
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
        this.GetDemandNotice(null);
    }

    GetDemandNotice(yr: number) {
        if (!isNullOrUndefined(yr)) {
            this.searchModel.dateYear = yr;
        }
        this.isLoading = true;
        this.demandnoticeservice.searchDemandNotice(this.searchModel, this.pageModel).subscribe(
            response => {
                const objschema = { data: [], totalPageCount: 0 };
                const res = Object.assign(objschema, response);
                this.demandNoticeLst = res.data.map(t => new DemandNoticeTaxpayer(t));
                this.pageModel.totalPageCount = res.totalPageCount;
                this.isLoading = false;
            },
            error => {
                this.isLoading = false;
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

    getRaisedRequest(batchId: string) {
        // this.isLoading = true;
        this.demandnoticeservice.getRaisedRequest(batchId)
            .subscribe(response => {
                //  this.isLoading = false;
                this.dowloadRequestList = response;
            }, error => {
                // this.isLoading = false;
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

    promptAction(params: string) {
        this.actionSelected = params.toLowerCase();
        let lt = this.demandNoticeLst.filter(b => b.isChecked == true).map(x => x.id);
        if (lt.length <= 0) {
            this.toasterService.pop('error', 'Error', 'Please select taxpayer!!!');
            return;
        }
        this.actionSelected = params.toLowerCase();
        jQuery(this.promptRequest.nativeElement).modal('show');
    }

    submitAction() {
        let lt = this.demandNoticeLst.filter(b => b.isChecked == true).map(x => x.id);
        if (this.actionSelected === "ARREARS".toLowerCase()) {
            this.addArrears(lt);
        } else if (this.actionSelected === "REMOVE ARREARS".toLowerCase()) {
            this.removeArrears();
        }  else if (this.actionSelected === "PENALTY".toLowerCase()) {
            this.runPenalty(lt);
        } else if (this.actionSelected === "DOWNLOAD".toLowerCase()) {
            this.downloadDNByTaxpayer(lt);
        } else {
            this.toasterService.pop('warning', 'No Action confirmed', 'Please refresh the page and try again');
        }
    }

    CheckAll() {
        this.masterCheck = !this.masterCheck;
        for (let i = 0; i < this.demandNoticeLst.length; i++) {
            const model: DemandNoticeTaxpayer = this.demandNoticeLst[i];
            this.demandNoticeLst[i].isChecked = this.masterCheck;
        }
    }

    addArrears(lt) {
        let payload = this.demandNoticeLst
        .filter(b => b.isChecked == true && b.isRunArrears == false).map(x => x.id);
        if (payload.length <= 0) {
            this.toasterService.pop('warning','Warning','No record found');
            return;
        }
        this.isLoadingMini = true;
        this.demandnoticeservice.runArrears(lt)
            .subscribe(response => {
                this.isLoadingMini = false;
                jQuery(this.promptRequest.nativeElement).modal('hide');                
                if (response.code === '00') {
                    this.toasterService.pop('success', 'Successful', response.description);
                    this.searchDemandNotice();
                } else {
                    this.toasterService.pop('error', 'Error', response.description);
                }
            }, error => {
                this.isLoadingMini = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    removeArrears() {
        let payload = this.demandNoticeLst
        .filter(b => b.isChecked == true && b.isRunArrears == true).map(x => x.id);

        if (payload.length <= 0) {
            this.toasterService.pop('warning','Warning','No record found');
            return;
        }
        
        this.isLoadingMini = true;
        this.demandnoticeservice.removeArrears(payload)
            .subscribe(response => {
                this.isLoadingMini = false;
                jQuery(this.promptRequest.nativeElement).modal('hide');
                if (response.code === '00') {
                    this.toasterService.pop('success', 'Successful', response.description);
                    this.searchDemandNotice();
                } else {
                    this.toasterService.pop('error', 'Error', response.description);
                }
            }, error => {
                this.isLoadingMini = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    runPenalty(lt) {
        if (this.searchModel.period <= 0) {
            this.toasterService.pop('error', 'Error', 'Arrears period is required');
            return;
        }

    }

    downloadDNByTaxpayer(lt) {

    }
    navigateView() {
        if (isNullOrUndefined(this.searchModel.wardId) &&
            isNullOrUndefined(this.searchModel.streetId)
            && isNullOrUndefined(this.searchModel.searchByName)
            && this.searchModel.dateYear <= 0) {
            return;
        }
        this.router.navigateByUrl('demandnotice/dnoticeview/' + btoa(JSON.stringify(this.searchModel)));

        // this.router.navigate(['demandnotice', 'dnoticeview'], { queryParams: { qry: btoa(JSON.stringify(this.searchModel)) } })
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.demandNoticeLst.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.GetDemandNotice(null);
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.GetDemandNotice(null);
    }

}