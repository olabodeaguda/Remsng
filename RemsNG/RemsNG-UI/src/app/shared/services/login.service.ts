import { DataService } from './data.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { LoginModel } from '../../login/models/login.model';

@Injectable()
export class LoginService {

    constructor(private dataService: DataService) {
    }

    GetUserDomain(username: string): Observable<Response> {
        return this.dataService.get('domain/domainByusername/' + username).catch(err => this.dataService.handleError(err));
    }

    SignIn(loginModel: LoginModel): Observable<Response> {
        this.dataService.addToHeader('value', btoa(JSON.stringify(loginModel)));
        return this.dataService.post('user', {}).catch(err => this.dataService.handleError(err));
    }
}
