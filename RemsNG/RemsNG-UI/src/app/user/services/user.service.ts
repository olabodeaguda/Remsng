import {Injectable} from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { DataService } from '../../shared/services/data.service';
import { PageModel } from '../../shared/models/page.model';
import { ProfileModel } from '../models/profile.model';
import { ChangePasswordModel } from '../models/change-password.model';
import { AssignDomainModel } from '../models/assign-domain.model';
import { AssignRoleModel } from '../models/assig-role.model';

@Injectable()
export class UserService {

    constructor(private dataService: DataService) {

    }

    getProfile(pageModel: PageModel): Observable<Response> {
        this.dataService.addToHeader('pageSize', pageModel.pageSize.toString());
        this.dataService.addToHeader('pageNum', pageModel.pageNum.toString());
      return this.dataService.get('user/profiles').catch(error => this.dataService.handleError(error));
    }

    add(user: ProfileModel) {
        return this.dataService.post('user/add', {
            email: user.email,
            username: user.username,
            surname: user.surname,
            firstname: user.firstname,
            gender: user.gender,
            lastname: user.lastname
        }).catch(error=>this.dataService.handleError(error));
    }

    update(user: ProfileModel) {
        return this.dataService.post('user/update', {
            email: user.email,
            username: user.username,
            surname: user.surname,
            firstname: user.firstname,
            lastname: user.lastname,
            gender: user.gender,
            id: user.id
        }).catch(error=>this.dataService.handleError(error));
    }

    changeStatus(user: ProfileModel) {
        return this.dataService.post('user/changestatus', {
            lcdaStatus: user.userStatus,
            id: user.id
        }).catch(error => this.dataService.handleError(error));
    }

    changePwd(user: ProfileModel, changePwd: ChangePasswordModel) {
        const obj: string = JSON.stringify( {
            newPwd: changePwd.newPwd,
            confirmPwd: changePwd.confirmPwd,
            id: user.id
        });
        this.dataService.addToHeader('value', btoa(obj))
        return this.dataService.post('user/changepwdchange', {}).catch(error => this.dataService.handleError(error));
    }
    
}