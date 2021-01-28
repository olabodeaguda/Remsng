import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DemandNoticeSearch } from '../models/demand-notice.search';
import { DemandNoticeService } from '../services/demand-notice.service';
import { ToasterService } from 'angular2-toaster';
import { DemandNoticeViewModel } from '../models/demand-notice-viewmodel';
import { ResponseModel } from '../../shared/models/response.model';
@Component({
    selector: 'app-dn-view',
    templateUrl: '../views/demand-notice-view.component.html'
})

export class DemandNoticeViewComponent implements OnInit {

    masterCheck: boolean = false;
    taxpayers = [];
    searchModel: DemandNoticeSearch = new DemandNoticeSearch();
    isLoading: boolean = false;
    savebatchLoading = false;
    constructor(private activeRouter: ActivatedRoute,
        private demandnoticeService: DemandNoticeService,
        private toasterService: ToasterService) {
    }

    ngOnInit() {
        let r: string = atob(this.activeRouter.snapshot.params['qry']);
        this.searchModel = JSON.parse(r);
        this.getQuery();
    }

    getQuery() {
        this.isLoading = true;
        this.demandnoticeService.searchInfo(this.searchModel)
            .subscribe(response => {
                this.isLoading = false;
                this.searchModel = <DemandNoticeSearch>response;
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', 'An error occur retrieving parameters information');
            });
    }

    getValidTaxpayers() {
        if(this.searchModel.period <= 0){
            this.toasterService.pop('warning','Error','Please select period');
            return;
        }

        this.isLoading = true;
        this.demandnoticeService.validTaxpayer(this.searchModel)
            .subscribe(response => {
                const t: ResponseModel = <ResponseModel>response;
                this.taxpayers = t.data.map(x => new DemandNoticeViewModel(x));
                this.isLoading = false;
            }, error => {
                this.isLoading = false;
            });
    }

    CheckAll() {
        this.masterCheck = !this.masterCheck;
        for (let i = 0; i < this.taxpayers.length; i++) {
            const model: DemandNoticeViewModel = this.taxpayers[i];
            if (model.itemCount > 0) {
                this.taxpayers[i].isChecked = this.masterCheck;
            }
        }
    }

    saveBatch() {
        this.searchModel.taxpayerIds = this.taxpayers.filter(b => b.isChecked == true).map(x => x.id);
        if (this.searchModel.taxpayerIds.length <= 0) {
            this.toasterService.pop('warning','Error','Taxpayer is required');
            return;
        }
        if(this.searchModel.period <= 0){
            this.toasterService.pop('warning','Error','Please select period');
            return;
        }
        this.isLoading = true;
        this.demandnoticeService.add2(this.searchModel)
            .subscribe(response => {
                this.isLoading = false;
                if (response.code == '00') {
                    this.toasterService.pop('success', 'Successful', response.description);
                    this.getValidTaxpayers();
                } else {                    
                    this.toasterService.pop('error', 'Error', response.description);
                }
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }
}