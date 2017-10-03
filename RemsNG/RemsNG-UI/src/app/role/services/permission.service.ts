import { Injectable } from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { PageModel } from "../../shared/models/page.model";


@Injectable()
export class PermissionService {

    constructor(private dataService: DataService) {
    }

    getPermissionByROleId(roleId: string, pageModel: PageModel) {
        this.dataService.addToHeader("pageNum", pageModel.pageNum.toString());
        this.dataService.addToHeader("pageSize", pageModel.pageSize.toString());
        return this.dataService.get('permission/byRoleid/' + roleId).catch(error => this.dataService.handleError(error));
    }

    getPermissionNotInRole(id: string) {
        return this.dataService.get('permissionnotinrole/'+id).catch(error => this.dataService.handleError(error));
    }

    


}