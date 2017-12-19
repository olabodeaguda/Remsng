import { UserModel } from '../models/user.model';
import { AppSettings } from '../models/app.settings';
import { Injectable, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class StorageService {
    appsettings: AppSettings;
    usermodelEmit = new EventEmitter<any>();
    constructor(private router: Router) {
        this.appsettings = new AppSettings();
    }
    remove() {
        const val = localStorage.getItem(this.appsettings.tk);
        if (val === null) {
           // window.location.replace('/login');
            this.router.navigate(['login'])
        } else {
            localStorage.removeItem(this.appsettings.tk);
        }
        const usermodel: UserModel = new UserModel();
        usermodel.fullname = 'Anonymous';
        this.usermodelEmit.emit(usermodel);  
        // window.location.replace('/login');
         this.router.navigate(['login'])      
    }
    
    updateToken(tk:string){
        let um:UserModel = this.get();
        um.tk = tk;
        this.Save(um);
    }

    Save(usermodel: UserModel) {
        const val: string = localStorage.getItem(this.appsettings.tk);
        if (val != null) {
            localStorage.removeItem(this.appsettings.tk);
        }
        this.usermodelEmit.emit(usermodel);
        localStorage.setItem(this.appsettings.tk, btoa(JSON.stringify(usermodel)));
    }

    get(): UserModel {
        const d = localStorage.getItem(this.appsettings.tk);
        if (d === null) {
            return null;
        }
        const user = atob(d);
        const um: UserModel = JSON.parse(user);
        return um;
    }

}
