import {Component} from '@angular/core';
import {LoginModel} from '../../shared/models/login.model';
import { Validators, FormBuilder, FormGroup, FormControl } from '@angular/forms';
import {Router} from '@angular/router';

@Component({
    selector: 'login',
    templateUrl: '../views/login.component.html'
})

export class LoginComponent {
    loginModel: LoginModel;
    signInForm: FormGroup;
    constructor(private router: Router) {
        this.loginModel = {
            username: '',
             pwd: ''
        };
        this.signInForm = new FormGroup({
            username: new FormControl('', Validators.required),
            pwd: new FormControl('', Validators.required)
        });
    }

    signIn() {
       this.loginModel = this.signInForm.value as LoginModel;
        // naviagete to dashboard
        this.router.navigateByUrl('/dashboard');
    }
}