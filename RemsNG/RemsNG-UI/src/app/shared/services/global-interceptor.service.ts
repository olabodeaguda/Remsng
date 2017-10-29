import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from "rxjs/Rx";
import { StorageService } from "./storage.service";

@Injectable()
export class GlobalInterceptorService implements HttpInterceptor {
  
  constructor(private storageservice:StorageService) {
    
  }
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next
      .handle(req)
      .do(event => {
        if (event instanceof HttpResponse) {
          if(event.headers.has('new-t')){
            const token:string = event.headers.get('new-t');
            this.storageservice.updateToken(token);
          }         
        }
      });
  }
}