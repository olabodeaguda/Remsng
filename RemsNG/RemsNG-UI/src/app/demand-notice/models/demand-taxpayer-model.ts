export class DemandNoticeTaxpayer {
    addressName: string;
    billingNumber: string;
    billingYr: string;
    councilTreasurerMobile: string;
    councilTreasurerSigFilen: string;
    createdBy: string;
    dateCreated: string;
    demandNoticeStatus: string;
    dnId: string;
    domainName: string;
    id: string;
    isUnbilled: string;
    lcdaAddress: string;
    lcdaLogoFileName: string;
    lcdaName: string;
    lcdaState: string;
    period: string;
    revCoodinatorSigFilen: string;
    streetName: string;
    taxpayerId: string;
    taxpayersName: string;
    wardName: string;
    isChecked : false;

    constructor(data: any) {
        this.addressName = data.addressName;
        this.billingNumber = data.billingNumber;
        this.billingYr = data.billingYr;
        this.councilTreasurerMobile = data.councilTreasurerMobile;
        this.councilTreasurerSigFilen = data.councilTreasurerSigFilen;
        this.createdBy = data.createdBy;
        this.dateCreated = data.dateCreated;
        this.demandNoticeStatus = data.demandNoticeStatus;
        this.dnId = data.dnId;
        this.domainName = data.domainName;
        this.id = data.id;
        this.isUnbilled = data.isUnbilled;
        this.lcdaAddress = data.lcdaAddress;
        this.lcdaLogoFileName = data.lcdaLogoFileName;
        this.lcdaName = data.lcdaName;
        this.lcdaState = data.lcdaState;
        this.period = data.period;
        this.revCoodinatorSigFilen = data.revCoodinatorSigFilen;
        this.streetName = data.streetName;
        this.taxpayerId = data.taxpayerId;
        this.taxpayersName = data.taxpayersName;
        this.wardName = data.wardName;
    }
}