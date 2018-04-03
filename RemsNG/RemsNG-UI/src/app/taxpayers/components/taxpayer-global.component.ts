import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { TaxpayerModel } from "../models/taxpayer.model";
import { StreetService } from "../../street/services/street.service";
import { StreetModel } from "../../street/models/street.model";
import { ActivatedRoute } from '@angular/router';
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { PageModel } from "../../shared/models/page.model";
import { TaxpayerService } from "../services/taxpayer.service";
import { CompanyService } from "../../company/services/company.service";
import { ResponseModel } from "../../shared/models/response.model";
import { ItemService } from "../../items/services/item.service";
import { WardService } from '../../ward/services/ward.service';
declare var jQuery: any;

@Component({
    selector: 'app-taxpayer',
    templateUrl: '../views/taxpayer-global.component.html'
})

export class TaxPayerGlobalComponent implements OnInit {
    lcdaId = '';
    companies = [];
    streets = [];
    taxpayerModel: TaxpayerModel;
    taxpayers = [];
    streetModel: StreetModel;
    isLoading: boolean = false;
    pageModel: PageModel;
    wardlst = [];
    @ViewChild('addModal') addModal: ElementRef;
    isAddLoading:boolean = false;
    query:string = '';

    constructor(private activeRoute: ActivatedRoute,
        private streetservice: StreetService,
        private taxpayerservice: TaxpayerService,
        private companyservice: CompanyService,
        private toasterService: ToasterService,
        private itemservice: ItemService, private wardService: WardService) {
        this.taxpayerModel = new TaxpayerModel();
        this.streetModel = new StreetModel();
        this.pageModel = new PageModel();
    }

    ngOnInit(): void {
        this.initilaizePage();
    }

    initilaizePage() {
        this.activeRoute.params.subscribe((param: any) => {
            this.lcdaId = param["id"];
            this.getTaxpayersByLcda();
            this.getWard();
            this.getCompanies();
        });
    }

    searchQuery() {
        if(this.query.length < 1){
            return;
        }

        this.isLoading = true;
        this.taxpayerservice.search(this.lcdaId, this.query).subscribe(response => {
            this.isLoading = false;
            this.taxpayers = response;
            this.pageModel.pageNum=1;
            this.pageModel.totalPageCount=1;
        }, error => {
            this.isLoading = false;
        })

    }

    wardChanges(wardId:string){
        this.getStreet(wardId);
    }

    getStreet(wardId:string) {
        if (wardId.length < 1) {
            return
        }

        this.isAddLoading = true;
        this.streetservice.byWardId(wardId).subscribe(response => {
            this.isAddLoading = false;
            this.streets = response;
        }, error => {
            this.isAddLoading = false;
        })
    }

    getWard() {
        this.isLoading = true;
        this.wardService.all().subscribe(response => {
            this.wardlst = response;
            this.isLoading = false;
        }, error => {
            this.isLoading = false;
        });
    }

    getCompanies() {
        this.isLoading = true;
        this.companyservice.byLgda(this.lcdaId).subscribe(response => {
            this.isLoading = false;
            this.companies = Object.assign([], response);
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    getTaxpayersBystreet() {
        if (this.streetModel.id.length < 1) {
            return
        }

        this.isLoading = true;
        this.taxpayerservice.byStreet(this.streetModel.id, this.pageModel).subscribe(response => {
            this.isLoading = false;
            const objSchema = { data: [], totalPageCount: 1 }
            const result = Object.assign(objSchema, response);
            this.taxpayers = result.data;
            this.pageModel.totalPageCount = result.totalPageCount;
        }, error => {
            this.isLoading = false;
        })
    }

    getTaxpayersByLcda() {
        if (this.lcdaId.length < 1) {
            return
        }

        this.isLoading = true;
        this.taxpayerservice.byLcda(this.lcdaId, this.pageModel).subscribe(response => {
            this.isLoading = false;
            const objSchema = { data: [], totalPageCount: 1 }
            const result = Object.assign(objSchema, response);
            this.taxpayers = result.data;
            this.pageModel.totalPageCount = result.totalPageCount;
        }, error => {
            this.isLoading = false;
        })
    }

    open(eventType: string, data: any) {
        if (eventType == 'ADD') {
            this.taxpayerModel = new TaxpayerModel();
            this.taxpayerModel.streetId = this.streetModel.id;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType == 'EDIT') {
            this.taxpayerModel = data;
            console.log(data);
            jQuery(this.addModal.nativeElement).modal('show');
        }
        this.taxpayerModel.eventType = eventType;
    }

    actions() {
        if (this.taxpayerModel.companyId.length < 1) {
            this.toasterService.pop('error', 'Error', 'Company is required');
            return;
        } else if (this.taxpayerModel.streetId.length < 1) {
            this.toasterService.pop('error', 'Error', 'Street is required');
            return;
        } else if (this.taxpayerModel.streetNumber.length < 1) {
            this.toasterService.pop('error', 'Error', 'Street number is required');
            return;
        }

        this.taxpayerModel.isLoading = true;
        if (this.taxpayerModel.eventType == 'ADD') {
            this.taxpayerservice.add(this.taxpayerModel).subscribe(response => {
                this.taxpayerModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.getTaxpayersByLcda();
                    this.toasterService.pop('success', 'SUCCESS', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                } else if (result.code == '20') {
                    const res: boolean = confirm(result.description + '. Are you sure');
                    if (res) {
                        this.taxpayerModel.isConfirmCompany = false;
                        this.actions();
                    }
                }
            }, error => {
                jQuery(this.addModal.nativeElement).modal('hide');
                this.taxpayerModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
        } else if (this.taxpayerModel.eventType == 'EDIT') {
            this.taxpayerservice.update(this.taxpayerModel).subscribe(response => {
                this.taxpayerModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'SUCCESS', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                }
            }, error => {
                jQuery(this.addModal.nativeElement).modal('hide');
                this.taxpayerModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
        }
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.taxpayers.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getTaxpayersByLcda()
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getTaxpayersByLcda();
    }

}