export class RoleModel {
    id: string = '';
    roleName: string = '';
    roleStatus: string = '';
    domainId: string = '';
    isErrMsg: boolean = false;
    errMsg: string = '';
    eventType: string = '';
    errClass: string[] = [];
    isLoading: boolean = false;
}