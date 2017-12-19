export class RoleModel {
    id: string = '';
    roleName: string = 'Nil';
    roleStatus: string = '';
    domainId: string = '';
    isErrMsg: boolean = false;
    errMsg: string = '';
    eventType: string = '';
    errClass: string[] = [];
    isLoading: boolean = false;
    domainName: string = '';
}