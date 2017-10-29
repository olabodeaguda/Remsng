import {Injectable} from '@angular/core';
import { PageModel } from '../../shared/models/page.model';
import { DataService } from '../../shared/services/data.service';
import { Observable } from 'rxjs/Rx';
import { DomainModel } from '../models/domain.model';

@Injectable()
export class DomainService {

    constructor(private dataService: DataService) {
    }

    all(pageModel: PageModel) {
        this.dataService.addToHeader('pageSize', pageModel.pageSize.toString());
        this.dataService.addToHeader('pageNum', pageModel.pageNum.toString());
       return this.dataService.get('domain/all').catch(
            error => this.dataService.handleError(error));
    }

    CurrentDomain() {
       return this.dataService.get('domain/currentdomain').catch(
            error => this.dataService.handleError(error));
    }

    activeDomains(){
       return this.dataService.get('domain/activeDomain').catch(
            error => this.dataService.handleError(error));
    }

    add(domainModel: DomainModel) {
        return this.dataService.post('domain/create', {
            domainName: domainModel.domainName,
            domainCode: domainModel.domainCode
        }).catch(error => this.dataService.handleError(error));
    }

    edit(domainModel: DomainModel) {
        return this.dataService.post('domain/update', {
            domainName: domainModel.domainName,
            domainCode: domainModel.domainCode,
            id: domainModel.id
        }).catch(error => this.dataService.handleError(error));
    }

    changeStatus(domainModel: DomainModel) {
        return this.dataService.post('domain/changestatus', {
            domainStatus: domainModel.domainStatus,
            id: domainModel.id
        }).catch(error => this.dataService.handleError(error));
    }
}
