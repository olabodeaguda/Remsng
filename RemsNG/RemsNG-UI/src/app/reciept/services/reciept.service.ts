import {Injectable} from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { PageModel } from '../../shared/models/page.model';

@Injectable()
export class RecieptService{

    constructor(private dataService:DataService) {
    }

    byLcda(pageModel:PageModel){
        this.dataService.addToHeader('pageNum', pageModel.pageNum.toString());
        this.dataService.addToHeader('pageSize', pageModel.pageSize.toString());
        return this.dataService.get('payment/bylcda').
        catch(error => this.dataService.handleError(error));
    }

    approvePayment(id:string,status:string){
        this.dataService.addToHeader('pmt', status);
        return this.dataService.post('payment/changestatus/'+id,{})
        .catch(error => this.dataService.handleError(error))
    }
    byBillingNumber(billingNumber:string){
        return this.dataService.get('payment/'+billingNumber)
        .catch(error=> this.dataService.handleError(error))
    }
}