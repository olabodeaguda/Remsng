import { Component, OnInit } from '@angular/core';
import { DemandNoticeSearch } from "../models/demand-notice.search";
import { AppSettings } from "../../shared/models/app.settings";
import { PageModel } from "../../shared/models/page.model";
import { DemandNoticeService } from "../services/demand-notice.service";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { WardService } from "../../ward/services/ward.service";
import { StreetService } from "../../street/services/street.service";

@Component({
    selector: 'demand-notice',
    templateUrl: '../views/demand-notice.component.html'
})

export class DemandNoticeComponent implements OnInit {

    searchModel: DemandNoticeSearch;
    wardLst = [];
    streetLst = [];
    yrLst = [];
    demandNoticeLst = [];
    pageModel: PageModel;
    isLoading: boolean = false;

    constructor(private appsettings: AppSettings,
        private demandnoticeservice: DemandNoticeService,
        private toasterService: ToasterService,
        private wardservice: WardService,
        private streetservice: StreetService) {
        this.searchModel = new DemandNoticeSearch();
        this.pageModel = new PageModel();
    }

    ngOnInit(): void {
        this.yrLst = this.appsettings.getYearList();
        this.getDemandNotice();
        this.getWards();
    }

    submitDemandRequest() {
        if (this.searchModel.dateYear <= 0) {
            this.toasterService.pop('error', 'Error', 'Biling year is required')
            return;
        }

        this.searchModel.isLoading = true;
        this.demandnoticeservice.add(this.searchModel).subscribe(
            response => {
                this.searchModel.isLoading = false;
                if (response.code === '00') {
                    this.toasterService.pop('success', 'Success', response.description);
                    this.getDemandNotice();
                } else {
                    this.toasterService.pop('error', 'Error', response.description);
                }
            },
            error => {
                this.searchModel.isLoading = false;
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