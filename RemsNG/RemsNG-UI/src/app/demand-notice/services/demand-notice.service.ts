import { Injectable } from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { PageModel } from "../../shared/models/page.model";
import { DemandNoticeSearch } from "../models/demand-notice.search";

@Injectable()
export class DemandNoticeService {

    constructor(private datataservice: DataService) {
    }

    get(pageModel: PageModel) {
        this.datataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.datataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        return this.datataservice.get('demandnotice/bylcda').catch(error => this.datataservice.handleError(error));
    }

    add(searchModel: DemandNoticeSearch) {
        let s = {
            wardId: searchModel.wardId,
            streetId: searchModel.streetId.length <= 0 ? null : searchModel.streetId,
            searchByName: null,
            dateYear: searchModel.dateYear <= 0 ? null : searchModel.dateYear,
            lcdaId: null
        };

        return this.datataservice.post('demandnotice', s).catch(x => this.datataservice.handleError(x));
    }
}