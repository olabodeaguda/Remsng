import { selector } from "rxjs/operator/publish";

export class DemandNoticePaymentModel{
    ownerId:string = '';
    billingNumber:string = '';
    amount:Number = 0;
    charges:Number = 0
    paymentMode:string = '';
    referenceNumber:string = '';
    bankId:string='';
    paymentStatus:string = '';
    createdBy:string = '';
    dateCreated:string = ''
}