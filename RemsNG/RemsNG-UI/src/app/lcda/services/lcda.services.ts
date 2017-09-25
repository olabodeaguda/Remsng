import {Injectable} from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { Observable } from 'rxjs/Observable';
import { PageModel } from '../../shared/models/page.model';
import { LcdaModel } from '../models/lcda.models';

@Injectable()
export class LcdaService {

    constructor(private dataService: DataService) {
    }

    all() {
        return this.dataService.get('lcda/total').catch(
            error => this.dataService.handleError(error));
    }

    getLcda(pageModel: PageModel): Observable<Response> {
        this.dataService.addToHeader('pageSize', pageModel.pageSize.toString());
        this.dataService.addToHeader('pageNum', pageModel.pageNum.toString());
       return this.dataService.get('lcda/all').catch(
            error => this.dataService.handleError(error));
    }

    addLCDA(lcdaModel: LcdaModel): Observable<Response> {
       return this.dataService.post('lcda/create', {
           domainId: lcdaModel.domainId,
           lcdaName: lcdaModel.lcdaName,
           lcdaCode: lcdaModel.lcdaCode
       }).catch(error => this.dataService.handleError(error));
    }

    editLCDA(lcdaModel: LcdaModel): Observable<Response> {
        return this.dataService.post('lcda/update', {
            domainId: lcdaModel.domainId,
            lcdaName: lcdaModel.lcdaName,
            lcdaCode: lcdaModel.lcdaCode,
            id: lcdaModel.id
        }).catch(error => this.dataService.handleError(error));
     }

     changeStatusLCDA(lcdaModel: LcdaModel): Observable<Response> {
        return this.dataService.post('lcda/changestatus', {
            lcdaStatus: lcdaModel.lcdaStatus,
            id: lcdaModel.id
        }).catch(error => this.dataService.handleError(error));
     }
}
