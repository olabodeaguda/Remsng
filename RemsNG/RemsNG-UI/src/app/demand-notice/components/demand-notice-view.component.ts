import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DemandNoticeSearch } from '../models/demand-notice.search';
import { DemandNoticeService } from '../services/demand-notice.service';
import { ToasterService } from 'angular2-toaster';
@Component({
    selector: 'app-dn-view',
    templateUrl: '../views/demand-notice-view.component.html'
})

export class DemandNoticeViewComponent implements OnInit {


    searchModel: DemandNoticeSearch;
    isLoading: boolean = false;
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


}