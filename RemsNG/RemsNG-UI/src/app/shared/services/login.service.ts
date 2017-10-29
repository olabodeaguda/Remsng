import { DataService } from './data.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { LoginModel } from '../../login/models/login.model';
import { HttpResponse } from "@angular/common/http";

@Injectable()
export class LoginService {

    constructor(private dataService: DataService) {
    }

    GetUserDomain(username: string) {
        return this.dataService.getWithoutHeader('lcda/byusername/' + username).catch(err => this.dataService.handleError(err));
    }

    SignIn(loginModel: LoginModel): Observable<Response> {
        if (loginModel.domainId === '') {
            this.dataService.addToHeader('value', btoa(JSON.stringify({
                 username: loginModel.username,
                 pwd: loginModel.pwd
            })));
        } else {
            this.dataService.addToHeader('value', btoa(JSON.stringify({
                username: loginModel.username,
                pwd: loginModel.pwd,
                domainId: loginModel.domainId
           })));
        }
        return this.dataService.postWithoutHeader('user', {}).catch(err => this.dataService.handleError(err));
    }
}
