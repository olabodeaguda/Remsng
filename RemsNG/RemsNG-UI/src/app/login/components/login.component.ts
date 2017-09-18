import { Component } from '@angular/core';
import { LoginModel } from '../models/login.model';
import { Validators, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginService } from '../../shared/services/login.service';
import { ResponseModel } from '../../shared/models/response.model';
import { StorageService } from '../../shared/services/storage.service';
import { UserModel } from '../../shared/models/user.model';
import { AppSettings } from '../../shared/models/app.settings';

@Component({
    selector: 'app-login',
    templateUrl: '../views/login.component.html'
})

export class LoginComponent {
    loginModel: LoginModel = new LoginModel();

    constructor(private router: Router, private loginService: LoginService,
        private storageService: StorageService, private appsettings: AppSettings) {
    }

    signIn() {
        this.loginModel.isLoading = true;
        this.loginService.SignIn(this.loginModel).subscribe(response => {
            setTimeout(() => {
                this.loginModel.isLoading = false;
            }, 2000);
            const result = Object.assign(new ResponseModel(), response.json());
            if (result.code === '00') {
                const usermodel: UserModel = Object.assign(new UserModel(), result.data);
                this.storageService.Save(usermodel);
                this.router.navigate(['/dashboard']);
            } else {
                this.loginModel.isError = true;
                this.loginModel.errmsg = result.description;
                setTimeout(() => {
                    this.loginModel.isError = false;
                }, 2000);
            }
        }, error => {
            this.loginModel.isLoading = false;
            this.loginModel.isError = true;
            this.loginModel.errmsg = error;
            setTimeout(() => {
                this.loginModel.isError = false;
            }, 2000);
        });
    }

    validateUsername() {
        if (this.loginModel.username.trim().length < 1) {
            this.loginModel.isError = true;
            this.loginModel.errmsg = 'Username is required';
            setTimeout(() => {
                this.loginModel.isError = false;
                this.loginModel.errmsg = '';
            }, 2000);
            return;
        }
        this.loginModel.isLoading = true;
        this.loginService.GetUserDomain(this.loginModel.username)
            .subscribe(
            response => {
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00' && result.data.length > 0) {
                    this.loginModel.isUsernameValid = true;
                    this.loginModel.domainIds = result.data;
                } else {
                    this.loginModel.isError = true;
                    this.loginModel.errmsg = result.description;
                    setTimeout(() => {
                        this.loginModel.isError = false;
                        this.loginModel.errmsg = '';
                        this.loginModel.errorClass.pop();
                    }, 4000);
                }
                this.loginModel.isLoading = false;
            },
            error => {
                this.loginModel.isLoading = false;
                this.loginModel.isError = true;
                this.loginModel.errmsg = error;
                this.loginModel.errorClass.push(this.appsettings.danger);
                setTimeout(() => {
                    this.loginModel.isError = false;
                    this.loginModel.errmsg = '';
                    this.loginModel.errorClass.pop();
                }, 2000);
            });
    }
}
