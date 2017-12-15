import { DataService } from './data.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { LoginModel } from '../../login/models/login.model';
import { HttpResponse } from "@angular/common/http";

@Injectable()
export class StateService {

    constructor(private dataService: DataService) {
    }

    GetStates() {
        return this.dataService.getWithoutHeader('state')
        .catch(err => this.dataService.handleError(err));
    }
}