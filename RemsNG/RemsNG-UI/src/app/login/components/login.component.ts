import { Component } from '@angular/core';
import { LoginModel } from '../models/login.model';
import { Validators, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
    selector: 'app-login',
    templateUrl: '../views/login.component.html'
})

export class LoginComponent {
    loginModel: LoginModel = new LoginModel();

    constructor(private router: Router) {
    }

    signIn() {
        this.router.navigateByUrl('/dashboard');
    }

    validateUsername() {
        setTimeout(() => {
            this.loginModel.isError = true;
        }, 1000);

        if (this.loginModel.username.trim().length < 1) {
            // Toast service
            return;
        }
        this.loginModel.isLoading = true;
    }
}
