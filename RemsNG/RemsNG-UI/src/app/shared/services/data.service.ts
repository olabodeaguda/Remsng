import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';
import { Router } from '@angular/router';
import { ToasterService } from 'angular2-toaster';
import { AppSettings } from '../models/app.settings';
import { ResponseModel } from '../models/response.model';
import { StorageService } from './storage.service';
import { UserModel } from '../models/user.model';

@Injectable()
export class DataService {

    public headers: Headers;
    public options: RequestOptions;

    constructor(private http: Http, private router: Router,
        private appConfig: AppSettings, private toasterService: ToasterService,
        private storageService: StorageService) {
        this.headers = new Headers({});
    }

    addToHeader(key: string, value: string) {
        if (this.headers.get(key) != null) {
            this.headers.delete(key);
        }
        this.headers.append(key, value);
    }

    initialize() {
        const tk: UserModel = this.storageService.get();
        if (tk !== null) {
            this.addToHeader('Authorization', 'Bearer ' + tk.tk);
        }

        this.options = new RequestOptions({
            responseType: ResponseContentType.Json,
            headers: this.headers
        });
    }

    get(url): Observable<Response> {
        this.initialize();
        return this.http.get(this.appConfig.BASE_URL + url, this.options);
    }

    post(url, body): Observable<Response> {
        this.initialize();
        return this.http.post(this.appConfig.BASE_URL + url, body, this.options);
    }

    put(url, body): Observable<Response> {
        this.initialize();
        return this.http.put(this.appConfig.BASE_URL + url, body, this.options);
    }

    delete(url): Observable<Response> {
        this.initialize();
        return this.http.delete(this.appConfig.BASE_URL + url, this.options);
    }

    translateResponse(result: any): ResponseModel {
        return Object.assign(new ResponseModel(), result);
    }

    handleError(err: any) {
        console.log(err);      
        const res = Object.assign(new ResponseModel(), err._body);
        if (err.status === 404) {
            return Observable.throw(res.description || 'Not found exception');
        } else if (err.status === 401) {
            return Observable.throw(res.description || 'You have no access to the selected page');
        } else if (err.status === 403) {
            this.toasterService.pop('error', res.description || 'You have no access to the selected page');           
            if (res.code === '09' || res.code == '10' || res.code === '11') {
                this.storageService.remove();
            }
            return Observable.throw(res.description || 'You have not access to the selected page');
        } else if (err.status == 500) {            
            //this.toasterService.pop('error', res.description || 'Internal server error occur. Please contact administrator');
            return Observable.throw(res.description || 'You have not access to the selected page');
        } else if (err.status == 0) { 
            this.storageService.remove();
            return Observable.throw(res.description || 'Connection to the server failed');
        } else if (err.status == 400) {
            //this.toasterService.pop('error', res.description || 'Internal server error occur. Please contact administrator');
            return Observable.throw(res.description || 'Internal server error occur. Please contact administrator');
        } else if (err.status == 409) {
            //this.toasterService.pop('error', res.description || 'Internal server error occur. Please contact administrator');
            return Observable.throw(res.description || 'Internal server error occur. Please contact administrator');
        } else {
            return Observable.throw(res.description || 'Connection to the server failed');
        }
    }
}
