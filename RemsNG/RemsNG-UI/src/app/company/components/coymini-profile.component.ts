import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CompanyModel } from "../models/company.model";
import { SectorService } from "../../sector/services/sector.services";
import { CategoryService } from "../../Category/services/category.service";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { CompanyService } from "../services/company.service";
import { ResponseModel } from "../../shared/models/response.model";

@Component({
    selector: 'coy-mini-profile',
    templateUrl: '../views/coymini-profile.component.html'
})

export class CoyMiniProfileComponent implements OnChanges {

    @Input()
    companyModel: CompanyModel;
    sectors = [];
    categories = [];
    isLoading: boolean = false;
    isDisabled: boolean = true;

    constructor(private sectorService: SectorService,
        private categoryservice: CategoryService,
        private toasterService: ToasterService,
        private companyservice: CompanyService) {

    }

    ngOnChanges(changes: SimpleChanges): void {
        this.getSectorbyLcda();
        this.getCategoryByLCda();
        this.companyModel.eventType = "EDIT";
    }

    getSectorbyLcda() {
        if (this.companyModel.lcdaId.length < 1) {
            return;
        }

        this.isLoading = true;
        this.sectorService.getSectorByLcdaId(this.companyModel.lcdaId)
            .subscribe(response => {
                this.isLoading = false;
                this.sectors = response.json();
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })
    }

    getCategoryByLCda() {
        if (this.companyModel.lcdaId.length < 1) {
            return;
        }

        this.isLoading = true;
        this.categoryservice.getAll(this.companyModel.lcdaId).subscribe(response => {
            this.isLoading = false;
            this.categories = response.json();
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        });
    }

    getCompany(coyId: string,isToggle: boolean) {
        this.companyservice.ById(coyId).subscribe(response => {
            this.companyModel = Object.assign(new CompanyModel(), response.json());
            if(isToggle){
                this.toggle();
            }
        }, error => {
            this.toasterService.pop('error', 'Error', error);
        })
    }


    toggle() {
        //disable all form or enable all form
        this.isDisabled = !this.isDisabled;
        if (this.isDisabled) {
            this.companyModel.eventType = "EDIT";
        } else {
            this.companyModel.eventType = "CANCEL";
        }
    }

    actions() {
        if (this.companyModel.lcdaId.length < 1) {
            this.toasterService.pop('error', 'Error', 'Please log out then login again and retry');
            return;
        } else if (this.companyModel.companyName.length < 1) {
            this.toasterService.pop('error', 'Error', 'Company Name is required');
            return;
        } else if (this.companyModel.categoryId.length < 1) {
            this.toasterService.pop('error', 'Error', 'Category is required');
            return;
        } else if (this.companyModel.sectorId.length < 1) {
            this.toasterService.pop('error', 'Error', 'Sector is required');
            return;
        }

        this.companyModel.isLoading = true;
        if (this.companyModel.eventType === 'EDIT' || this.companyModel.eventType === 'CANCEL') {
            this.companyservice.update(this.companyModel).subscribe(response => {
                this.companyModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    this.getCompany(this.companyModel.id, true);                    
                }
            }, error => {
                this.companyModel.isLoading = false;
            });
        }
    }
}