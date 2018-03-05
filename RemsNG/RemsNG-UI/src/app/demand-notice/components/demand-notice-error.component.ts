import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DemandNoticeService } from '../services/demand-notice.service';
import { ErrorModel } from '../../shared/models/error.model';

@Component({
    selector: 'Demand Notice Error Details',
    templateUrl: '../views/demand-notice-error.component.html'
})

export class DemandNoticeErrorComponent implements OnInit {

    errorModel: ErrorModel[] = [];
    constructor(private activeRoute: ActivatedRoute,
        private demandNoticeService: DemandNoticeService) {
    }

    ngOnInit() {
        // this.activeRoute.params.subscribe((param: any) => {
        //     this.loadDemandNoticeError(param["id"]);
        // });
    }

    // loadDemandNoticeError(id: string) {
    //     this.demandNoticeService.demandNoticeError(id)
    //     .subscribe(reponse=>{
    //        // this.errorModel = Object.assign(ErrorModel[],)
    //     }, error =>{

    //     })
    // }
}
