import { Injectable } from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { PageModel } from "../../shared/models/page.model";
import { TaxpayerModel } from "../models/taxpayer.model";

@Injectable()
export class TaxpayerService {

    constructor(private dataservice: DataService) {
    }
    byLcda(lcdaId: string, pageModel: PageModel) {
        this.dataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.dataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        return this.dataservice.get('bylcdapaginated/' + lcdaId).catch(x => this.dataservice.handleError(x));
    }

    byStreet(lcdaId: string, pageModel: PageModel) {
        this.dataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.dataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        return this.dataservice.get('taxpayer/bystreetpaginated/' + lcdaId).catch(x => this.dataservice.handleError(x));
    }
    add(taxpayer: TaxpayerModel){
        this.dataservice.addToHeader('confirmcompany',taxpayer.isConfirmCompany?'true':'false');
        return this.dataservice.post('taxpayer',{
            streetId: taxpayer.streetId,
            companyId: taxpayer.companyId,
            streetNumber: taxpayer.streetNumber,
            surname:taxpayer.surname,
            firstname:taxpayer.firstname,
            lastname: taxpayer.lastname
        }).catch(x=> this.dataservice.handleError(x));
    }

    update(taxpayer: TaxpayerModel){
        return this.dataservice.put('taxpayer',taxpayer).catch(x=> this.dataservice.handleError(x));
    }
}