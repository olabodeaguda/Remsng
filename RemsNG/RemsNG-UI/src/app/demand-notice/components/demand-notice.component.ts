import { Component, OnInit } from '@angular/core';
import { DemandNoticeSearch } from "../models/demand-notice.search";
import { AppSettings } from "../../shared/models/app.settings";
import { PageModel } from "../../shared/models/page.model";

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

    constructor(private appsettings: AppSettings) {
        this.searchModel = new DemandNoticeSearch();
        this.pageModel = new PageModel();
    }

    ngOnInit(): void {
        this.yrLst = this.appsettings.getYearList();
    }

    action() {
        
    }

    getDemandNotice(){

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