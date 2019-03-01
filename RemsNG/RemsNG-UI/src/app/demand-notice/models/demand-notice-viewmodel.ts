export class DemandNoticeViewModel{
    id: string;
    taxpayerName: string
    itemCount: number
    isChecked = false;
    demandNoticeStatus: string;
    taxpayerStatus: string;
    constructor(data: any){
        this.id = data.id;
        this.itemCount = data.itemCount;
        this.taxpayerName = `${data.surname} ${data.firstname} ${data.lastname}`;
        this.demandNoticeStatus = data.demandNoticeStatus;
        this.taxpayerStatus = data.taxpayerStatus;
    }
}