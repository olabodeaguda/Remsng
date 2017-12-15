import {Injectable} from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { PageModel } from '../../shared/models/page.model';
import { DemandNoticeSearch } from '../models/demand-notice.search';
import { AmountDueModel } from '../models/amount-due.model';


@Injectable()
export class DemandNoticeTaxpayerService{

  
    constructor(private dataservice:DataService) {
        
    }

    byBatchId(batchId:string,pageModel:PageModel){
        this.dataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.dataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        return this.dataservice.get('dnt/batchno/'+batchId)
        .catch(error => this.dataservice.handleError(error));
    }

    
    ByBillingno(billingnumber:string){
        return this.dataservice.get('dnt/billingno/'+billingnumber)
        .catch(error => this.dataservice.handleError(error));
    }

    downloadRpt(url:string){
        return this.dataservice.getBlob('dndownload/single/'+url)       
        .catch(error => this.dataservice.handleError(error));
    }

    amountDueByBillingNo(billingnumber:string){
        return this.dataservice.get('amountdue/'+billingnumber)
        .catch(error => this.dataservice.handleError(error));
    }

    UpdateAmount(amountDueModel: AmountDueModel){
        return this.dataservice.post('amountdue',{
            id:amountDueModel.id,
            itemAmount:amountDueModel.itemAmount,
            category:amountDueModel.category
        }).catch(error => this.dataservice.handleError(error));
    }
    
}