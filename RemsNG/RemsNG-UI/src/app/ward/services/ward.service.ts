import {Injectable} from '@angular/core';
import { WardModel } from '../models/ward.model';
import { DataService } from '../../shared/services/data.service';
import { Observable } from 'rxjs/Observable';
import { PageModel } from '../../shared/models/page.model';

@Injectable()
export class WardService {

    constructor(private dataService: DataService) {
        
    }

    all() {
        return this.dataService.get('ward/all').catch(
            error => this.dataService.handleError(error));
    }

    getWard(pageModel: PageModel, id: string): Observable<Response> {
        this.dataService.addToHeader('pageSize', pageModel.pageSize.toString());
        this.dataService.addToHeader('pageNum', pageModel.pageNum.toString());
        this.dataService.addToHeader('lcdaId', id);
       return this.dataService.get('ward/paginated').catch(
            error => this.dataService.handleError(error));
    }

    addWard(wardModel: WardModel): Observable<Response> {
        return this.dataService.post('ward',{
            wardName: wardModel.wardName,
            lcdaId: wardModel.lcdaId
        }).catch(error => this.dataService.handleError(error));
    }

    editWard(wardModel: WardModel): Observable<Response> {
        return this.dataService.post('ward/update', {
            wardName: wardModel.wardName,
            lcdaId: wardModel.lcdaId,
            id: wardModel.id
        }).catch(error => this.dataService.handleError(error));
     }

     changeStatusWard(wardModel: WardModel): Observable<Response> {
        return this.dataService.post('ward/changestatus', {
            wardStatus: wardModel.wardStatus,
            id: wardModel.id
        }).catch(error => this.dataService.handleError(error));
     }

     byId(id: string): Observable<Response>{
         return this.dataService.get('ward/'+id).catch(x=> this.dataService.handleError(x));
     }

}
