import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { LcdaService } from "../../lcda/services/lcda.services";
import { SectorService } from "../../sector/services/sector.services";
import { CategoryService } from "../../Category/services/category.service";
import { CompanyService } from "../services/company.service";
import { ActivatedRoute } from '@angular/router';
import { LcdaModel } from "../../lcda/models/lcda.models";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { PageModel } from "../../shared/models/page.model";
import { CompanyModel } from "../models/company.model";
declare var jQuery: any;

@Component({
    selector: 'app-company',
    templateUrl: '../views/company.component.html'
})

export class CompanyComponent implements OnInit {
    lcdaModel: LcdaModel;
    isLoading: boolean = false;
    sectors = [];
    categories = [];
    companies = [];
    pageModel: PageModel;
    companyModel: CompanyModel;
    @ViewChild('addModal') addModal: ElementRef;

    constructor(private activeRoute: ActivatedRoute, private lcdaservice: LcdaService,
        private sectorService: SectorService,
        private categoryservice: CategoryService,
        private companyservice: CompanyService,
        private toasterService: ToasterService) {
        this.lcdaModel = new LcdaModel();
        this.pageModel = new PageModel();
        this.companyModel = new CompanyModel();
    }

    ngOnInit(): void {
        this.activeRoute.params.subscribe((param: any) => {
            this.getLcda(param["id"]);
        });
    }

    getCompanyByLcda() {
        if (this.lcdaModel.id.length < 1) {
            return;
        }
        this.isLoading = true;
        this.companyservice.byLcda(this.lcdaModel.id, this.pageModel)
            .subscribe(response => {
                this.isLoading = false;
                const result = response;
                const resultScheme = { data: [], totalPageCount: 0 };
                const responseD = Object.assign(resultScheme, result);
                this.companies = responseD.data;
                this.pageModel.totalPageCount = responseD.totalPageCount;
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })
    }

    getLcda(lcdaId: string) {
        this.isLoading = true;
        this.lcdaservice.getLCdaById(lcdaId).subscribe(response => {
            this.isLoading = false;
            const objSchema = Object.assign(new ResponseModel(), response);
            if (objSchema.code == '00') {
                this.lcdaModel = objSchema.data;
                this.getSectorbyLcda();
                this.getCategoryByLCda();
                this.getCompanyByLcda();
            }
            else {
                this.toasterService.pop('error', 'Error', objSchema.desciption);
            }

        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    getSectorbyLcda() {
        this.isLoading = true;
        this.sectorService.getSectorByLcdaId(this.lcdaModel.id)
            .subscribe(response => {
                this.isLoading = false;
                this.sectors = response;
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })
    }

    getCategoryByLCda() {
        if (this.lcdaModel.id.length < 1) {
            return;
        }

        this.isLoading = true;
        this.categoryservice.getAll(this.lcdaModel.id).subscribe(response => {
            this.isLoading = false;
            this.categories = response;
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        });
    }

    open(eventType: string, data) {
        if (eventType === 'ADD') {
            this.companyModel = new CompanyModel();
            this.companyModel.lcdaId = this.lcdaModel.id;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === 'EDIT') {
            this.companyModel = data;
            jQuery(this.addModal.nativeElement).modal('show');
        }

        this.companyModel.eventType = eventType;
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
        if (this.companyModel.eventType === 'ADD') {
            this.companyservice.add(this.companyModel).subscribe(response => {
                this.companyModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getCompanyByLcda();
                }
            }, error => {
                this.companyModel.isLoading = false;
            });
        }
        else if(this.companyModel.eventType === 'EDIT'){
            this.companyservice.update(this.companyModel).subscribe(response => {
                this.companyModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getCompanyByLcda();
                }
            }, error => {
                this.companyModel.isLoading = false;
            });
        }
    }
}
