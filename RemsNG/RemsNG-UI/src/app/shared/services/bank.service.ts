import { DataService } from './data.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { LoginModel } from '../../login/models/login.model';
import { HttpResponse } from "@angular/common/http";

@Injectable()
export class BankService {

    constructor(private dataService: DataService) {
    }

    getBanks() {
        return this.dataService.get('banks')
        .catch(err => this.dataService.handleError(err));
    }
}