export class TaxpayerModel{
    id: string = '';
    companyId:string = '';
    streetId: string = '';
    addressid: string = '';
    taxpayerStatus: string = '';
    companyName: string = '';
    dateCreated: string = '';
    createdBy: string = '';
    lastmodifiedby: string = '';
    lastModifiedDate: string = '';
    streetNumber: string = '';
    eventType:string = '';
    isLoading:boolean = false;
    isConfirmCompany:boolean = true;
    surname: string ='';
    firstname: string ='';
    lastname: string ='';
    wardId: string = '';
    isOneTime: boolean = false;    
    isChecked = false;
  
    constructor(data: any){
        this.id = data.id;
        this.companyId = data.companyId;
        this.streetId=data.streetId;
        this.addressid=data.addressid;        
        this.taxpayerStatus = data.taxpayerStatus;
        this.companyName = data.companyName;
        this.dateCreated = data.dateCreated;
        this.createdBy = data.createdBy;
        this.lastmodifiedby= data.lastmodifiedby;
        this.streetNumber= data.streetNumber;
        this.eventType = data.eventType;
        this.surname=data.surname;
        this.firstname=data.firstname;
        this.lastname = data.lastname;
        this.wardId= data.wardId;
        this.isOneTime=data.isOneTime;
        this.lastModifiedDate = data.lastModifiedDate;
    }
}