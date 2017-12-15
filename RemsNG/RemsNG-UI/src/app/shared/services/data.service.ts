import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { Router } from '@angular/router';
import { ToasterService } from 'angular2-toaster';
import { AppSettings } from '../models/app.settings';
import { ResponseModel } from '../models/response.model';
import { StorageService } from './storage.service';
import { UserModel } from '../models/user.model';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Headers, RequestOptions, ResponseContentType } from '@angular/http';

@Injectable()
export class DataService {

    public options: RequestOptions;
    public map: Map<string, string> = new Map<string, string>();

    constructor(private http: HttpClient, private router: Router,
        private appConfig: AppSettings, private toasterService: ToasterService,
        private storageService: StorageService) {
    }

    addToHeader(key: string, value: string) {
        if (this.map.get(key) != null) {
            this.map.delete(key);
        }

        this.map.set(key, value);
    }

    addBearer() {
        const tk: UserModel = this.storageService.get();
        this.addToHeader('Access-Control-Expose-Headers', '\"*\"');
        if (tk !== null) {
            this.addToHeader('Authorization', 'Bearer ' + tk.tk);
        }
    }

    getHeader() {
        let headerObj = {};
        this.map.forEach((v: string, k: string) => {
            headerObj[k] = v;
        });
        return headerObj;
    }

    getWithoutHeader(url): Observable<object> {
        return this.http.get(this.appConfig.BASE_URL + url, {
            headers: new HttpHeaders(this.getHeader())
        });
    }

    get(url): Observable<object> {
        this.addBearer();
        return this.http.get(this.appConfig.BASE_URL + url, {
            headers: new HttpHeaders(this.getHeader())
        });
    }

    getBlob(url): Observable<any> {
        const tk: UserModel = this.storageService.get();
       
        const options = {
            headers: new HttpHeaders({
                'Authorization': 'Bearer ' + tk.tk
            })
            , responseType: 'blob' as 'blob'
        };

        return this.http.get((this.appConfig.BASE_URL + url), options);
    }

    post(url, body): Observable<object> {
        this.addBearer();
        return this.http.post(this.appConfig.BASE_URL + url, body, {
            headers: new HttpHeaders(this.getHeader())
        });
    }

    postWithoutHeader(url, body): Observable<object> {
        return this.http.post(this.appConfig.BASE_URL + url, body, {
            headers: new HttpHeaders(this.getHeader())
        });
    }

    put(url, body): Observable<object> {
        this.addBearer();
        return this.http.put(this.appConfig.BASE_URL + url, body, {
            headers: new HttpHeaders(this.getHeader())
        });
    }

    delete(url): Observable<object> {
        this.addBearer();
        return this.http.delete(this.appConfig.BASE_URL + url, {
            headers: new HttpHeaders(this.getHeader())
        });
    }

    translateResponse(result: any): ResponseModel {
        return Object.assign(new ResponseModel(), result);
    }

    handleError(err: any) {
        console.log(err.error);
        const res = Object.assign(new ResponseModel(), err.error);
        if (err.status === 404) {
            return Observable.throw(res.description || 'Not found exception');
        } else if (err.status === 401) {
            if (res.code === '09' || res.code == '10' || res.code === '11') {
                this.toasterService.pop('error', res.description || 'You have no access to the selected page');
                this.storageService.remove();
            }
            return Observable.throw(res.description || 'You have no access to the selected page');
        } else if (err.status === 403) {
            if (res.code === '09' || res.code == '10' || res.code === '11') {
                this.toasterService.pop('error', res.description || 'You have no access to the selected page');
                this.storageService.remove();
            }
            return Observable.throw(res.description || 'You have not access to the selected page');
        } else if (err.status == 500) {
            return Observable.throw(res.description || 'You have not access to the selected page');
        } else if (err.status == 0) {
            //  this.storageService.remove();
            return Observable.throw(res.description || 'Connection to the server failed');
        } else if (err.status == 400) {
            return Observable.throw(res.description || 'Internal server error occur. Please contact administrator');
        } else if (err.status == 409) {
            return Observable.throw(res.description || 'Internal server error occur. Please contact administrator');
        } else {
            return Observable.throw(res.description || 'Connection to the server failed');
        }
    }


}
