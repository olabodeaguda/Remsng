import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { isNullOrUndefined } from 'util';
import { TaxpayerService } from '../services/taxpayer.service';
import { ToasterService } from 'angular2-toaster';
import * as FileSaver from 'file-saver'

@Component({
    selector: 'taxpayer-payment',
    templateUrl: '../views/taxpayer.payment.component.html'
})
export class TaxpayerPayerHistory implements OnInit {

    constructor(private activateRoute: ActivatedRoute,
        private _taxServ: TaxpayerService,
        private toasterService: ToasterService) {
    }

    taxpayers = [];
    taxpayer;
    isLoading = false;
    ngOnInit() {
        const id = this.activateRoute.snapshot.params['id'];
        if (!isNullOrUndefined(id)) {
            this.getTaxpayer(id);
        }
    }

    getTaxpayer(taxpayerId){
        this.isLoading = true;
        this._taxServ.getTaxpayerId(taxpayerId)
            .subscribe(response => {
                this.isLoading = false;
                this.taxpayer = response;
                this.getHistory(taxpayerId);
            }, error => {
                this.isLoading = false;
                const res = error.error;
                this.toasterService.pop('error', 'Error', res.description);
            })
    }

    getHistory(taxpayerId) {
        this.isLoading = true;
        this._taxServ.getPaymentHistory(taxpayerId)
            .subscribe(response => {
                this.isLoading = false;
                this.taxpayers = response;
            }, error => {
                this.isLoading = false;
                const res = error.error;
                this.toasterService.pop('error', 'Error', res.description);
            });
    }

    downloadDN(url: string) {
        this.isLoading = true;
        this._taxServ.downloadRpt(url).map(response => {
              this.isLoading = false;
             let blob = response;
             FileSaver.saveAs(blob,url+".pdf");
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error',"Download Error",error);
        }).subscribe();
        
    }

    raisePenalty(id: string){
        this.isLoading = true;
        this._taxServ.raisePenalty(id)
            .subscribe(response => {
                this.isLoading = false;
                this.toasterService.pop('success', 'Success', response.description);
            }, error => {
                this.isLoading = false;
                const res = error.error;
                this.toasterService.pop('error', 'Error', res.description);
            });
    }
}