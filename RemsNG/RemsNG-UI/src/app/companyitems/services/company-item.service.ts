import { Injectable } from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { CompanyItem } from "../models/company-item.model";

@Injectable()
export class ComponentItemService {

    constructor(private dataservice: DataService) {
    }

    getItemByCompany(companyId: string) {
        return this.dataservice.get('companyitem/bycompany/' + companyId).catch(x => this.dataservice.handleError(x));
    }

    add(companyitem: CompanyItem) {
        return this.dataservice.post('companyitem/', {

        }).catch(x => this.dataservice.handleError(x));
    }

    update(companyitem: CompanyItem) {
        return this.dataservice.put('companyitem', {
            
        }).catch(x => this.dataservice.handleError(x));
    }

    updateStatus(companystatus: string, id: string) {
        return this.dataservice.put('companyitem/changestatus', {
            id: id,
            companyStatus: companystatus
        }).catch(x => this.dataservice.handleError(x));
    }
}