import {Injectable} from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { Observable } from 'rxjs/Observable';
import { PageModel } from '../../shared/models/page.model';

@Injectable()
export class LcdaService {

    constructor(private dataService: DataService) {
    }

    getLcda(pageModel: PageModel): Observable<Response> {
        this.dataService.addToHeader('pageSize', pageModel.pageSize.toString());
        this.dataService.addToHeader('pageNum', pageModel.pageNum.toString());
       return this.dataService.get('domain/all').catch(
            error => this.dataService.handleError(error));
    }
}
