export class ItemPenaltyModel{
    id:string = '';
    itemId: string = '';
    isPercentage:boolean = false;
    penaltyStatus: string = '';
    amount = 0;
    duration: string;
    isLoading: boolean = false;
    eventType: string = '';
    currentstatus:string = '';
}