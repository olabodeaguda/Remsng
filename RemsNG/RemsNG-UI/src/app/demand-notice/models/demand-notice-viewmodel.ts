export class DemandNoticeViewModel{
    id: string;
    taxpayerName: string
    itemCount: number
    isChecked = false;
    constructor(data: any){
        this.id = data.id;
        this.itemCount = data.itemCount;
        this.taxpayerName = `${data.surname} ${data.firstname} ${data.lastname}`;
    }
}