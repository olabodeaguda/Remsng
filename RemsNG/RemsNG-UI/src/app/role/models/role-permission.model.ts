export class RolePermissionModel {
    permissionId: string;
    roleId: string;
    permissionName: string;
    isErrMsg: boolean = false;
    errMsg: string = '';
    eventType: string = '';
    errClass: string[] = [];
    isLoading: boolean = false;
}