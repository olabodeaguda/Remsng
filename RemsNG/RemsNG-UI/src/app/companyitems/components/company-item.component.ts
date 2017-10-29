import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ComponentItemService } from "../services/company-item.service";
import { PageModel } from "../../shared/models/page.model";
import { ActivatedRoute } from '@angular/router';
import { CompanyModel } from "../../company/models/company.model";
import { CompanyService } from "../../company/services/company.service";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { CompanyItem } from "../models/company-item.model";
import { ItemService } from "../../items/services/item.service";
import { AppSettings } from "../../shared/models/app.settings";
import { ResponseModel } from "../../shared/models/response.model";
declare var jQuery: any;

@Component({
    selector: 'company-item',
    templateUrl: '../views/company-item.component.html'
})

export class ComponentItemComponent implements OnInit {

    companyModel: CompanyModel;
    companyLst = [];
    pageModel: PageModel;
    isLoading: boolean = false;
    taxpayersId: string = '';
    streetId: string = '';
    companyItem: CompanyItem;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changeModal') changeModal: ElementRef;
    yrLst = [];
    items = [];
    constructor(private companyitemservice: ComponentItemService,
        private activeRoute: ActivatedRoute,
        private toasterService: ToasterService,
        private companyservice: CompanyService,
        private itemservice: ItemService, private appsettings: AppSettings) {
        this.pageModel = new PageModel();
        this.companyModel = new CompanyModel();
        this.companyItem = new CompanyItem();
    }

    ngOnInit(): void {
        this.yrLst = this.appsettings.getYearList();
        this.initialize();
    }

    initialize() {
        this.activeRoute.params.subscribe((param: any) => {
            this.taxpayersId = param['txid'];
            this.streetId = param['streetid'];
            this.getCompanyitemsByTaxPayers();
            this.getItemByTaxpayerId();
        });
    }

    open(eventType: string, data: any) {
        if (eventType === 'ADD' || eventType === 'EDIT') {
            if (eventType === 'ADD') {
                this.companyItem = new CompanyItem();
                this.companyItem.taxpayerId = this.taxpayersId;
            } else {
                this.companyItem = data;
            }

            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === 'CHANGE_STATUS') {
            this.companyItem = data;
            jQuery(this.changeModal.nativeElement).modal('show');
        }
        this.companyItem.eventType = eventType;
    }


    getCompanyitemsByTaxPayers() {
        if (this.taxpayersId.length < 1) {
            return;
        }

        this.isLoading = true;
        this.companyitemservice.getCompanyItemByTaxpayer(this.taxpayersId, this.pageModel)
            .subscribe(response => {
                this.isLoading = false;
                const obj = { data: [], totalPageCount: 0 };
                const r = Object.assign(obj, response);
                this.companyLst = r.data;
                this.pageModel.totalPageCount = r.totalPageCount;
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    getItemByTaxpayerId() {
        if (this.taxpayersId.length < 1) {
            return;
        }
        this.isLoading = true;
        this.itemservice.itemByTaxpayers(this.taxpayersId)
            .subscribe(response => {
                this.isLoading = false;
                this.items = Object.assign([], response);
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    actions() {
        if (this.companyItem.itemId.length < 1) {
            this.toasterService.pop('error', 'Error', 'Item is required!!!');
            return;
        } else if (this.companyItem.amount < 1) {
            this.toasterService.pop('error', 'Error', 'Amount is required!!!');
            return;
        } else if (this.companyItem.billingYear < 1) {
            this.toasterService.pop('error', 'Error', 'Billing year is required!!!');
            return;
        }

        this.companyItem.isLoading = true;
        if (this.companyItem.eventType === 'ADD') {
            this.companyitemservice.add(this.companyItem).subscribe(response => {
                this.companyItem.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code == '00') {
                    this.getCompanyitemsByTaxPayers();
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.toasterService.pop('success', 'Success', response.description);
                } else {
                    this.toasterService.pop('error', 'Errror', response.description);
                }
            }, error => {
                this.companyItem.isLoading = false
                jQuery(this.addModal.nativeElement).modal('hide');
            })
        } else if (this.companyItem.eventType === 'EDIT') {
            this.companyitemservice.update(this.companyItem).subscribe(response => {
                this.companyItem.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code == '00') {
                    this.getCompanyitemsByTaxPayers();
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.toasterService.pop('success', 'Success', result.description);
                } else {
                    this.getCompanyitemsByTaxPayers();
                    this.toasterService.pop('error', 'Errror', result.description);
                }
            }, error => {
                this.getCompanyitemsByTaxPayers();
                this.companyItem.isLoading = false
                jQuery(this.addModal.nativeElement).modal('hide');
            })
        } else if (this.companyItem.eventType === 'CHANGE_STATUS') {
            this.companyItem.companyStatus = this.companyItem.companyStatus === 'ACTIVE' ? 'NOT_ACTIVE' : 'ACTIVE';
            this.companyitemservice.updateStatus(this.companyItem.companyStatus, this.companyItem.id)
                .subscribe(response => {
                    this.companyItem.isLoading = false;                    
                    jQuery(this.changeModal.nativeElement).modal('hide');
                    this.getCompanyitemsByTaxPayers();
                    const result = Object.assign(new ResponseModel(), response);
                    if (result.code == '00') {
                        this.toasterService.pop('success', 'Success', result.description);
                    } else {
                        this.toasterService.pop('error', 'Errror', result.description);
                    }
                }, error => {
                    jQuery(this.changeModal.nativeElement).modal('hide');
                    this.getCompanyitemsByTaxPayers();
                    this.companyItem.isLoading = false
                    jQuery(this.addModal.nativeElement).modal('hide');
                })
        }
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.companyLst.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getCompanyitemsByTaxPayers();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }

        this.getCompanyitemsByTaxPayers();
    }
}