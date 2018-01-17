import { Injectable } from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { PageModel } from '../../shared/models/page.model';
import { DemandNoticeSearch } from '../models/demand-notice.search';
import { retry } from 'rxjs/operators/retry';

@Injectable()
export class DemandNoticeService {

    constructor(private datataservice: DataService) {
    }

    get(pageModel: PageModel) {
        this.datataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.datataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        return this.datataservice.get('demandnotice/bylcda').
        catch(error => this.datataservice.handleError(error));
    }

    get2(pageModel: PageModel) {
        this.datataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.datataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        return this.datataservice.get('demandnotice/bylcda');
    }

    bybatchId(batchno:string){
        return this.datataservice.get('demandnotice/batchno/'+batchno).catch(error => this.datataservice.handleError(error));
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

    adDownloadRequest(batchno: string){
        return this.datataservice.post('dndownload/'+batchno, {})
        .catch(x => this.datataservice.handleError(x));
    }

    getRaisedRequest(batchno:string){
        return this.datataservice.get('dndownload/'+batchno)
        .catch(x => this.datataservice.handleError(x));
    }

    downloadRpt(url:string){
        return this.datataservice.getBlob('dndownload/bulk/'+url)       
        .catch(error => this.datataservice.handleError(error));
    }

    addArrears(data: any) {
        const outD: any = {
            billingNo: data.billingNumber,
            taxpayerId: data.id,
            totalAmount: data.itemAmount,
            itemId: data.itemId
        };

        return this.datataservice.post('demandnotice/addarrears', outD).catch(x => this.datataservice.handleError(x));
    }

}
