import { Injectable } from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { PageModel } from "../../shared/models/page.model";

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
}