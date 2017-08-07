import {Component} from '@angular/core';
import {LoginModel} from '../../shared/models/login.model';

@Component({
    selector: 'login',
    templateUrl: '../views/login.component.html'
})

export class LoginComponent {
    loginModel: LoginModel;

    constructor() {
        this.loginModel = new LoginModel();
    }

    signIn() {
        alert('ok');
    }

}