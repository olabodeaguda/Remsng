import { Injectable } from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { CompanyItem } from "../models/company-item.model";
import { PageModel } from "../../shared/models/page.model";

@Injectable()
export class ComponentItemService {

    constructor(private dataservice: DataService) {
    }

    getCompanyItemByTaxpayer(taxpayerId: string,pageModel:PageModel) {
        this.dataservice.addToHeader('pageNum',pageModel.pageNum.toString());
        this.dataservice.addToHeader('pageSize',pageModel.pageSize.toString())
        return this.dataservice.get('companyitem/bytaxpayer/' + taxpayerId).catch(x => this.dataservice.handleError(x));
    }

    add(companyitem: CompanyItem) {
        return this.dataservice.post('companyitem/', {
            taxpayerId: companyitem.taxpayerId,
            itemId: companyitem.itemId,
            billingYear: companyitem.billingYear,
            amount: companyitem.amount
        }).catch(x => this.dataservice.handleError(x));
    }

    update(companyitem: CompanyItem) {
        return this.dataservice.put('companyitem', {
            taxpayerId: companyitem.taxpayerId,
            itemId: companyitem.itemId,
            billingYear: companyitem.billingYear,
            amount: companyitem.amount,
            id:companyitem.id
        }).catch(x => this.dataservice.handleError(x));
    }

    updateStatus(companystatus: string, id: string) {
        return this.dataservice.post('companyitem/changestatus', {
            id: id,
            companyStatus: companystatus
        }).catch(x => this.dataservice.handleError(x));
    }
}