import {Injectable} from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { PageModel } from '../../shared/models/page.model';


@Injectable()
export class DemandNoticeTaxpayerService{

  
    constructor(private dataservice:DataService) {
        
    }

    byBatchId(batchId:string,pageModel:PageModel){
        this.dataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.dataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        return this.dataservice.get('dnt/batchno/'+batchId)
        .catch(error => this.dataservice.handleError(error));
    }

    downloadRpt(url:string){
        return this.dataservice.getBlob('dndownload/single/'+url)       
        .catch(error => this.dataservice.handleError(error));
    }
    
}