import { Component } from '@angular/core';
import { LoginModel } from '../models/login.model';
import { Validators, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginService } from '../../shared/services/login.service';
import { ResponseModel } from '../../shared/models/response.model';
import { StorageService } from '../../shared/services/storage.service';
import { UserModel } from '../../shared/models/user.model';
import { AppSettings } from '../../shared/models/app.settings';
import { DomainModel } from '../../domain/models/domain.model';

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
        if(this.loginModel.username !== this.loginModel.validatedUsername){
            this.loginModel = new LoginModel();
            return;
        }
        this.loginModel.isLoading = true;
        this.loginService.SignIn(this.loginModel).subscribe(response => {
            setTimeout(() => {
                this.loginModel.isLoading = false;
            }, 2000);
            const result = Object.assign(new ResponseModel(), response);
            if (result.code === '00') {
                const usermodel: UserModel = Object.assign(new UserModel(), result.data);
                this.storageService.Save(usermodel);
                this.router.navigate(['/dashboard']);
            } else {
                this.loginModel.isError = true;
                this.loginModel.errorClass = new Array(this.appsettings.danger);
                this.loginModel.errmsg = result.description;
                setTimeout(() => {
                    this.loginModel.errorClass.pop();
                    this.loginModel.isError = false;
                }, 2000);
            }
        }, error => {
            this.loginModel.isLoading = false;
            this.loginModel.isError = true;
            this.loginModel.errorClass = new Array(this.appsettings.danger);
            this.loginModel.errmsg = error;
            setTimeout(() => {
                this.loginModel.errorClass.pop();
                this.loginModel.isError = false;
            }, 2000);
        });
    }

    validateUsername() {
        if (this.loginModel.username.trim().length < 1) {
            this.loginModel.isError = true;
            this.loginModel.errmsg = 'Username is required';
            this.loginModel.errorClass.push(this.appsettings.danger);
            setTimeout(() => {
                this.loginModel.isError = false;
                this.loginModel.errmsg = '';
                this.loginModel.errorClass.pop();
            }, 2000);
            return;
        }
        this.loginModel.isLoading = true;
        this.loginService.GetUserDomain(this.loginModel.username)
            .subscribe(
            response => {
                this.loginModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.loginModel.isUsernameValid = true;
                    this.loginModel.validatedUsername = this.loginModel.username;
                    if (result.data.length > 1) {
                        this.loginModel.domainIds = result.data;
                    }else if (result.data.length == 1) {
                        const dModel = <DomainModel>result.data[0];
                        this.loginModel.domainId = dModel.id;
                    }
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
