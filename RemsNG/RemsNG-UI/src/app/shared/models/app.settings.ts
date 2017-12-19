import { Injectable } from '@angular/core';

@Injectable()
export class AppSettings {
    public tk: string = 'rems';
    public BASE_URL: string = 'api/v1/';

    public root_url: string = '/';
    // error class
    public success: string = 'alert alert-success';
    public info: string = 'alert alert-info';
    public warning: string = 'alert alert-warning';
    public danger: string = 'alert alert-danger';

    // eventType
    public editMode: string = 'EDIT';
    public addMode: string = 'ADD';
    public changeStatusMode: string = 'CHANGE_STATUS';
    public changePwdMode: string = 'CHANGE_PWD';
    public assignLGDA: string = 'ASSIGN_LGDA';
    public assignRole: string = 'ASSIGN_ROLE'
    public removeMode: string = 'REMOVE';
    public profileMode: string = 'PROFILE';
    public emailPattern: string = '.+\@.+\..+';

    public domainStatus: string[] = ['ACTIVE', 'NOT_ACTIVE'];

    validatEmail(value: string) {
        const reg = new RegExp(this.emailPattern);
        return reg.test(value);
    }

    validatePhoneNumber(value: string) {    //2347039555295
        const reg2: string = '^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{3,9}$';
        const reg4 = new RegExp(reg2);
        if (reg4.test(value)) {
            return true;
        }
        return false;
    }

    getYearList() {
        const initalNum: number = 2017;
        const nm: number = new Date().getFullYear();
        let s: number[] = [];

        for (let i: number = (nm + 1); i >= initalNum; i--) {
            s.push(i);
        }
        return s;
    }


}
