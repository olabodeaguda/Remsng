import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { DataService } from '../../shared/services/data.service';
import { PageModel } from '../../shared/models/page.model';
import { RoleModel } from '../models/role.model';
import { AssignRoleModel } from '../../user/models/assig-role.model';
import { RolePermissionModel } from "../models/role-permission.model";


@Injectable()
export class RoleService {

    constructor(private dataService: DataService) {

    }

    getRoles(pageModel: PageModel): Observable<Response> {
        this.dataService.addToHeader('pageSize', pageModel.pageSize.toString());
        this.dataService.addToHeader('pageNum', pageModel.pageNum.toString());
        return this.dataService.get('role').catch(
            error => this.dataService.handleError(error));
    }

    add(roleModel: RoleModel): Observable<Response> {
        return this.dataService.post('role', {
            roleName: roleModel.roleName,
            domainId: roleModel.domainId
        }).catch(error => this.dataService.handleError(error));
    }

    update(roleModel: RoleModel): Observable<Response> {
        return this.dataService.post('role/update', {
            roleName: roleModel.roleName,
            domainId: roleModel.domainId,
            id: roleModel.id
        }).catch(error => this.dataService.handleError(error));
    }

    changeStatus(roleModel: RoleModel): Observable<Response> {
        return this.dataService.post('role/changestatus', {
            roleStatus: roleModel.roleStatus,
            id: roleModel.id
        }).catch(error => this.dataService.handleError(error));
    }

    getAllDomainRoles(usrname: string): Observable<Response> {
        this.dataService.addToHeader('username', usrname);
        return this.dataService.get('role/alldomainroles').catch(
            error => this.dataService.handleError(error));
    }

    assignRoleTouser(assignRoleModel: AssignRoleModel): Observable<Response> {
        return this.dataService.post('role/assignrole', {
            userId: assignRoleModel.userId,
            roleId: assignRoleModel.roleId
        }).catch(error => this.dataService.handleError(error));
    }

    get(roleId: string) {
        return this.dataService.get('role/' + roleId).catch(error => this.dataService.handleError(error));
    }

    assignPermissionToRole(userperm: RolePermissionModel): Observable<Response> {
        return this.dataService.post('role/assignroletopermission', {
            roleId: userperm.roleId,
            permissionId: userperm.permissionId
        }).catch(error => this.dataService.handleError(error));
    }
    removePermission(userperm: RolePermissionModel): Observable<Response> {
        return this.dataService.post('role/removerolepermission', {
            roleId: userperm.roleId,
            permissionId: userperm.permissionId
        }).catch(error => this.dataService.handleError(error));
    }
}