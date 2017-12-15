import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DemandNoticeTaxpayerService } from '../services/demand-noticeTaxpayer.service';
import { DemandNoticeModel } from '../models/demand-notice.models';
import { PageModel } from '../../shared/models/page.model';
import { DemandNoticeService } from '../services/demand-notice.service';
import { error } from 'util';
import { ToasterService } from 'angular2-toaster';
import { DomSanitizer } from '@angular/platform-browser';
import { AppSettings } from '../../shared/models/app.settings';
import { DataService } from '../../shared/services/data.service';
import * as FileSaver from 'file-saver'
import { window } from 'rxjs/operators/window';
import { resetFakeAsyncZone } from '@angular/core/testing';
import { UserModel } from '../../shared/models/user.model';
import { StorageService } from '../../shared/services/storage.service';

@Component({
    selector: 'app-dnt',
    templateUrl: '../views/demand-noticeTaxpayers.component.html'
})

export class DemandNoticeTaxpayersComponent implements OnInit {

    private isLoading: boolean;
    private taxpayersLst = [];
    private pageModel: PageModel = new PageModel();
    downloadModel={fileBytes:'',filename:''};
    private demandNoticeModel: DemandNoticeModel = new DemandNoticeModel();
    constructor(private activeRoute: ActivatedRoute,
        private dtsService: DemandNoticeTaxpayerService,
        private dNotice: DemandNoticeService, private sanitizer: DomSanitizer,
        private appsettings: AppSettings, private toasterService: ToasterService,
        private storageService: StorageService) {
        this.isLoading = false;
    }
    ngOnInit() {
        this.initializePage();
    }
    initializePage() {
        this.activeRoute.params.subscribe((param: any) => {
            this.getDemandNoticeByBatchId(param["batchId"]);
        });
    }

    sanitize(url: string) {
        return this.sanitizer.bypassSecurityTrustUrl(this.appsettings.root_url + "/api/v1/dndownload/single/" + url);
    }

    getDemandNoticeByBatchId(batchno: string) {
        this.dNotice.bybatchId(batchno).subscribe(response => {
            if (response.code === '00') {
                this.demandNoticeModel = response.data;
                this.getTaxpayerBybatchId();
            }
        }, error => {

        })
    }

    getTaxpayerBybatchId() {
        if (this.demandNoticeModel.batchNo.length < 1) {
            return;
        }
        this.isLoading = false;
        this.dtsService.byBatchId(this.demandNoticeModel.batchNo, this.pageModel)
            .subscribe(response => {
                const objschema = { data: [], totalPageCount: 0 };
                const res = Object.assign(objschema, response);
                this.taxpayersLst = Object.assign([], response.data);
                this.pageModel.totalPageCount = res.totalPageCount;
                this.isLoading = false;
            }, error => {
                this.isLoading = false;
            });
    }

    downloadDN(url: string) {
        this.isLoading = true;
        this.dtsService.downloadRpt(url).map(response => {
              this.isLoading = false;
             let blob = response;
             FileSaver.saveAs(blob,url+".pdf");
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error',"Download Error",error);
        }).subscribe();
        
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.taxpayersLst.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getTaxpayerBybatchId();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getTaxpayerBybatchId();
    }
}