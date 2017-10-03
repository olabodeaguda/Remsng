import { Injectable } from '@angular/core';

@Injectable()
export class AppSettings {
    public tk: string = 'rems';
    public BASE_URL: string = 'http://localhost:54313/api/v1/';

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
    public removeMode: string = 'REMOVE'

    public domainStatus: string[] = ['ACTIVE', 'NOT_ACTIVE'];
}
