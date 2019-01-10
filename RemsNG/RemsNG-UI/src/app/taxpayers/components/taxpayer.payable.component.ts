import { OnInit, Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TaxpayerService } from '../services/taxpayer.service';
import { ToasterService } from 'angular2-toaster';
import { isNullOrUndefined } from 'util';
import * as FileSaver from 'file-saver'

@Component({
    selector:'app-payable',
    templateUrl:'../views/taxpayer.payable.component.html'
})
export class TaxpayerPayableComponent implements OnInit {
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
        this._taxServ.getTaxPayable(taxpayerId)
            .subscribe(response => {
                this.isLoading = false;
                this.taxpayers = response;
                console.log(this.taxpayer);
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
}
