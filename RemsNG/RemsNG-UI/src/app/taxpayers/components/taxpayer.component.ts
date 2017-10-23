import { Component, OnInit } from '@angular/core';
import { TaxpayerModel } from "../models/taxpayer.model";
import { StreetService } from "../../street/services/street.service";
import { StreetModel } from "../../street/models/street.model";
import { ActivatedRoute } from '@angular/router';
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { PageModel } from "../../shared/models/page.model";
import { TaxpayerService } from "../services/taxpayer.service";

@Component({
    selector: 'app-taxpayer',
    templateUrl: '../views/taxpayer.component.html'
})

export class TaxPayerComponent implements OnInit {

    taxpayerModel: TaxpayerModel;
    taxpayers = [];
    streetModel: StreetModel;
    isLoading: boolean = false;
    pageModel: PageModel;

    constructor(private activeRoute: ActivatedRoute,
        private streetservice: StreetService, private taxpayerservice: TaxpayerService,
        private toasterService: ToasterService) {
        this.taxpayerModel = new TaxpayerModel();
        this.streetModel = new StreetModel();
        this.pageModel = new PageModel();
    }

    ngOnInit(): void {
        this.initilaizePage();
    }

    initilaizePage() {
        this.activeRoute.params.subscribe((param: any) => {
            this.getStreet(param["id"]);
        });
    }

    getStreet(streetId: string) {
        if (streetId.length < 1) {
            return
        }
        this.isLoading = true;
        this.streetservice.byId(streetId).subscribe(response => {
            this.isLoading = false;
            this.streetModel = Object.assign(new StreetModel(), response.json());
            this.getTaxpayersBystreet();
        }, error => {
            this.isLoading = false;
        })
    }

    getTaxpayersBystreet() {
        if (this.streetModel.id.length < 1) {
            return
        }

        this.isLoading = true;
        this.taxpayerservice.byStreet(this.streetModel.id,this.pageModel).subscribe(response=>{
            this.isLoading = false;
            this.taxpayers = Object.assign([], response.json());
        }, error=>{
            this.isLoading = false;
        })
      

    }

    open(eventType: string, data: any) {

    }

    next() {
        if (this.pageModel.pageNum > 1 && this.taxpayers.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getTaxpayersBystreet();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getTaxpayersBystreet();
    }

}