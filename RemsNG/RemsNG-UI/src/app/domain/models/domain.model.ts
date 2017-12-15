export class DomainModel {
    id: string = '';
    domainName: string = '';
    domainCode: string = '';
    domainStatus: string = '';
    datecreated: string = '';
    stateId: string = '';
    isLoading: boolean = false;
    errClass: string[] = new Array<string>(1);
    msg: string = '';
    isErrMsg: boolean = false;
    eventType: string = '';
}
