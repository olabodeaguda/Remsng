import {Injectable} from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { Observable } from 'rxjs/Observable';
import { PageModel } from '../../shared/models/page.model';
import { LcdaModel } from '../models/lcda.models';

@Injectable()
export class LcdaService {

    constructor(private dataService: DataService) {
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
}
