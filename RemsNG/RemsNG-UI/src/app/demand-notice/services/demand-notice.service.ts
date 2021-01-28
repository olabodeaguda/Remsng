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

    bybatchId(batchno: string) {
        return this.datataservice.get('demandnotice/batchno/' + batchno).catch(error => this.datataservice.handleError(error));
    }

    add(searchModel: DemandNoticeSearch) {
        let s = {
            wardId: searchModel.wardId,
            streetId: searchModel.streetId.length <= 0 ? null : searchModel.streetId,
            searchByName: null,
            dateYear: searchModel.dateYear <= 0 ? null : searchModel.dateYear,
            lcdaId: null,
            CloseOldData: searchModel.isClosedData,
            RunArrears: searchModel.runArrears,
            IsUnbilled: searchModel.isUnbilled,
            RunPenalty: searchModel.runPenalty,
            RunArrearsCategory: searchModel.runArrearsCategory
        };

        return this.datataservice.post('demandnotice', s).catch(x => this.datataservice.handleError(x));
    }
    add2(searchModel: DemandNoticeSearch) {
        let s = {
            wardId: searchModel.wardId,
            streetId: searchModel.streetId.length <= 0 ? null : searchModel.streetId,
            searchByName: null,
            dateYear: searchModel.dateYear <= 0 ? null : searchModel.dateYear,
            lcdaId: null,
            RunArrears: searchModel.runArrears,
            IsUnbilled: searchModel.isUnbilled,
            RunPenalty: searchModel.runPenalty,
            RunArrearsCategory: searchModel.runArrearsCategory,
            useSingleBill: searchModel.useSingleBill,
            Period: +searchModel.period,
            taxpayerIds: searchModel.taxpayerIds
        };

        return this.datataservice.post('demandnotice/run/bytaxpayer', s).catch(x => this.datataservice.handleError(x));
    }

    searchDemandNotice(searchModel: DemandNoticeSearch, pageModel: PageModel) {
        this.datataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.datataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        let s: any = {
            wardId: searchModel.wardId,
            streetId: searchModel.streetId,
            searchByName: searchModel.searchByName,
            dateYear: searchModel.dateYear,
            lcdaId: null
        };

        return this.datataservice
            .post('demandnotice/search/' + pageModel.pageNum + '/' + pageModel.pageSize, s)
            .catch(x => this.datataservice.handleError(x));
    }

    
    searchDemandNotice1(searchModel: DemandNoticeSearch, pageModel: PageModel) {
        this.datataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.datataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        let s: any = {
            wardId: searchModel.wardId,
            streetId: searchModel.streetId,
            searchByName: searchModel.searchByName,
            dateYear: searchModel.dateYear,
            lcdaId: null
        };

        return this.datataservice
            .post('demandnotice/search', s)
            .catch(x => this.datataservice.handleError(x));
    }


    adDownloadRequest(batchno: string) {
        return this.datataservice.post('dndownload/' + batchno, {})
            .catch(x => this.datataservice.handleError(x));
    }

    getRaisedRequest(batchno: string) {
        return this.datataservice.get('dndownload/' + batchno)
            .catch(x => this.datataservice.handleError(x));
    }

    downloadRpt(url: string) {
        return this.datataservice.getBlob('dndownload/bulk/' + url)
            .catch(error => this.datataservice.handleError(error));
    }

    addArrears(data: any) {
        const outD: any = {
            id: data.dnId,
            amount: data.itemAmount,
            itemId: data.itemId
        };

        return this.datataservice.post('demandnotice/arrears/single/add', outD)
        .catch(x => this.datataservice.handleError(x));
    }

    cancelDemandNotice(billingNo: string) {
        return this.datataservice.get('dnt/cancel/' + billingNo)
        .catch(x => this.datataservice.handleError(x));
    }

    currentReport() {
        return this.datataservice.get('dnt/cancel')
        .catch(x => this.datataservice.handleError(x));
    }

    demandNoticeError(id: string) {
        return this.datataservice.get('dnt/error/' + id)
        .catch(x => this.datataservice.handleError(x));
    }

    MovetoBills(billno) {
        return this.datataservice.get('dnt/movetobill/' + billno)
        .catch(x => this.datataservice.handleError(x));
    }

    MovetoUnBills(billno) {
        return this.datataservice.get('dnt/movetoUnbill/' + billno)
        .catch(x => this.datataservice.handleError(x));
    }

    searchInfo(searchModel: DemandNoticeSearch) {
        let s: any = {
            wardId: searchModel.wardId,
            streetId: searchModel.streetId,
            searchByName: searchModel.searchByName,
            dateYear: searchModel.dateYear,
            lcdaId: null
        };
        return this.datataservice
            .post('demandnotice/searchinfo/', s)
            .catch(x => this.datataservice.handleError(x));
    }

    validTaxpayer(searchModel: DemandNoticeSearch) {
        let s: any = {
            wardId: searchModel.wardId,
            streetId: searchModel.streetId,
            searchByName: searchModel.searchByName,
            dateYear: searchModel.dateYear,
            lcdaId: null,
            runArrears: searchModel.runArrears,
            period: +searchModel.period
        };
        return this.datataservice
        .post('demandnotice/validtaxpayers/', s)
        .catch(x => this.datataservice.handleError(x));
    }

    runArrears(taxpayerIds){
        let r = JSON.stringify(taxpayerIds);
        return this.datataservice
        .post('demandnotice/arrears/add', taxpayerIds)
        .catch(x => this.datataservice.handleError(x));
    }
    
    removeArrears(taxpayerIds){
        let r = JSON.stringify(taxpayerIds);
        return this.datataservice
        .post('demandnotice/arrears/remove', taxpayerIds)
        .catch(x => this.datataservice.handleError(x));
    }

    runPenalty(taxpayerIds){
        let r = JSON.stringify(taxpayerIds);
        return this.datataservice
        .post('demandnotice/penalty/add', taxpayerIds)
        .catch(x => this.datataservice.handleError(x));
    }
    
    removePenalty(taxpayerIds){
        let r = JSON.stringify(taxpayerIds);
        return this.datataservice
        .post('demandnotice/penalty/remove', taxpayerIds)
        .catch(x => this.datataservice.handleError(x));
    }

    Delete(taxpayerIds){
        let r = JSON.stringify(taxpayerIds);
        return this.datataservice
        .post('dnt/delete', taxpayerIds)
        .catch(x => this.datataservice.handleError(x));
    }

    allArrears(searchModel: DemandNoticeSearch){
        let s: any = {
            wardId: searchModel.wardId,
            streetId: searchModel.streetId,
            searchByName: searchModel.searchByName,
            dateYear: searchModel.dateYear,
            lcdaId: null
        };
        return this.datataservice
        .post('demandnotice/arrears/add/all', s)
        .catch(x => this.datataservice.handleError(x));
    }

    allPenalty(searchModel: DemandNoticeSearch){
        let s: any = {
            wardId: searchModel.wardId,
            streetId: searchModel.streetId,
            searchByName: searchModel.searchByName,
            dateYear: searchModel.dateYear,
            lcdaId: null
        };
        return this.datataservice
        .post('demandnotice/penalty/add/all', s)
        .catch(x => this.datataservice.handleError(x));
    }

    downloadBulk(payload) {
        return this.datataservice.postBlob('dndownload/bulk/',payload)
            .catch(error => this.datataservice.handleError(error));
    }
    downloadReminderBulk(payload) {
        return this.datataservice.postBlob('dndownload/reminder/', payload)
            .catch(error => this.datataservice.handleError(error));
    }
}
