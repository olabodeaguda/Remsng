export class DemandNoticeSearch {
    lcdaId: string = '';
    wardId: string = '';
    streetId: string = '';
    searchByName: string = '';
    dateYear: number = 0;
    isLoading: boolean = false;
    isProcessingRequest: boolean = false;
    billingNo: string = '';
    isClosedData: boolean = false;
    runArrears: boolean = false;
    runArrearsCategory: number = -1;
    isUnbilled: boolean = false;
    runPenalty: boolean = false;
    period: string;
    useSingleBill: boolean = false;
}
