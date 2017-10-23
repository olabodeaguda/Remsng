import { Injectable } from '@angular/core';
import { CompanyModel } from "../models/company.model";
import { DataService } from "../../shared/services/data.service";
import { PageModel } from "../../shared/models/page.model";

@Injectable()
export class CompanyService {

    constructor(private dataservice: DataService) {
    }

    add(companyModel: CompanyModel) {
        return this.dataservice.post('company', {
            companyName: companyModel.companyName,
            lcdaId: companyModel.lcdaId,
            sectorId: companyModel.sectorId,
            categoryId: companyModel.categoryId
        }).catch(x => this.dataservice.handleError(x));
    }

    byLcda(id: string, pagemodel: PageModel) {
        this.dataservice.addToHeader('pageNum', pagemodel.pageNum.toString())
        this.dataservice.addToHeader('pageSize', pagemodel.pageSize.toString())
        return this.dataservice.get('company/bylcdapaging/' + id).catch(x => this.dataservice.handleError(x))
    }

    update(companyModel: CompanyModel) {
        return this.dataservice.put('company', {
            id: companyModel.id,
            companyName: companyModel.companyName,
            lcdaId: companyModel.lcdaId,
            sectorId: companyModel.sectorId,
            categoryId: companyModel.categoryId
        }).catch(x => this.dataservice.handleError(x));
    }

    ById(id: string){
        return this.dataservice.get('company/'+id).catch(x=> this.dataservice.handleError(x));
    }

}