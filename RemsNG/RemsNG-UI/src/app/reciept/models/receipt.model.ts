export class ReceiptModel{
    amount:number = 0;
    bankId:string = '';
    billingNumber:string = '';
    billingYear:number = 0;
    charges:number = 0;
    dateCreated:string = '';
    id:string = '';
    ownerId:string = '';
    paymentMode:string = '';
    paymentStatus:string = '';
    referenceNumber: string = '';
    newPaymentStatus:string = '';
    isLoading:boolean = false;
    IsWaiver = false;
}