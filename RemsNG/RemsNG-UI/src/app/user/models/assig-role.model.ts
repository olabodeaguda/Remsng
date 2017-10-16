import { RoleModel } from "../../role/models/role.model";

export class AssignRoleModel {
    userId: string = '';
    roleId: string = '';    
    isErrMsg: boolean = false;
    msg: string = '';
    isLoading: boolean = false;
    currentRole: RoleModel = new RoleModel();
    domainId: string = ''
}