import { Injectable } from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { DemandNoticePaymentModel } from '../models/demandNotice-payment.model';

@Injectable()
export class DemandNoticePaymentService {

    constructor(private dataservice: DataService) {
    }

    registerPayment(dnpModel: DemandNoticePaymentModel) {
        return this.dataservice.post('payment', {
            billingNumber:dnpModel.billingNumber,
            bankId:dnpModel.bankId,
            referenceNumber:dnpModel.referenceNumber,
            amount:dnpModel.amount
        }).catch(error => this.dataservice.handleError(error));
    }

    getRecipetList(billingNumber: string){
        return this.dataservice.get('payment/'+billingNumber)
        .catch(error => this.dataservice.handleError(error));
    }

}