import {Component, Input, OnInit} from '@angular/core';
import { CompanyModel } from "../models/company.model";
import { LcdaModel } from "../../lcda/models/lcda.models";
import { CompanyService } from "../services/company.service";
import { ActivatedRoute } from '@angular/router';
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { ResponseModel } from "../../shared/models/response.model";
import { LcdaService } from "../../lcda/services/lcda.services";
import { ProfileModel } from "../../user/models/profile.model";
declare var jQuery: any;

@Component({
    selector:'comppany-profile',
    templateUrl: '../views/company-profile.component.html'
})

export class CompanyProfileComponent implements OnInit{
    profileModel: ProfileModel;
    companyModel: CompanyModel;
    lcdaModel: LcdaModel;
    isLoading:boolean = false;
    constructor(private activeRoute: ActivatedRoute, 
        private companyservice: CompanyService,
        private toasterService: ToasterService,
        private lcdaservice: LcdaService) {
        this.companyModel = new CompanyModel();
        this.lcdaModel = new LcdaModel();
        this.profileModel = new ProfileModel();
    }

    ngOnInit(): void {
        this.activeRoute.params.subscribe((param: any) => {
            this.getCompany(param["id"]);
            this.profileModel.id = param["id"];
            this.profileModel.lcdaId = param["lcdaId"];
        });
    }

    getCompany(coyId: string){
        this.companyservice.ById(coyId).subscribe(response=>{
            this.companyModel = Object.assign(new CompanyModel(),response);          
            this.getLcda();
        },error=>{
            this.toasterService.pop('error','Error',error);
        })
    }

    getLcda() {
        if(this.companyModel.lcdaId.length < 1){
            return
        }
        this.isLoading = true;
        this.lcdaservice.getLCdaById(this.companyModel.lcdaId).subscribe(response => {
            this.isLoading = false;
            const objSchema = Object.assign(new ResponseModel(), response);
            if (objSchema.code == '00') {
                this.lcdaModel = objSchema.data;
            }
            else {
                this.toasterService.pop('error', 'Error', objSchema.desciption);
            }

        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

}