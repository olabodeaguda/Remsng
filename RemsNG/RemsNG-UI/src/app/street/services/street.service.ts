import {Injectable} from '@angular/core'
import { DataService } from "../../shared/services/data.service";
import { StreetModel } from "../models/street.model";
import { PageModel } from "../../shared/models/page.model";

@Injectable()
export class StreetService{

    constructor(private dataService: DataService) {
    }

    byWardId(wardid: string){
        return this.dataService.get('street/bywardid/'+wardid).catch(x=> this.dataService.handleError(x));
    }

    byWardIdpaginated(wardid: string, pageModel:PageModel){
        if(pageModel.pageNum === 0){
            pageModel.pageNum = 1;
        }
        this.dataService.addToHeader("pageNum", pageModel.pageNum.toString());
        this.dataService.addToHeader("pageSize", pageModel.pageSize.toString());
        return this.dataService.get('street/paginated/'+wardid).catch(x=> this.dataService.handleError(x));
    }

    byId(id: string){
        return this.dataService.get('street/'+id).catch(x=> this.dataService.handleError(x));
    }

    add(streetmodel:StreetModel){
        return this.dataService.post('street',{
            wardId: streetmodel.wardId,
            streetName: streetmodel.streetName,
            numberOfHouse: streetmodel.numberOfHouse,
            streetDescription: streetmodel.streetDescription
        }).catch(x=> this.dataService.handleError(x));
    }

    update(street: StreetModel){
        return this.dataService.put('street/'+street.id,street).catch(x=> this.dataService.handleError(x));
    }

    changeStatus(streetmodel:StreetModel){
        return this.dataService.delete('street/'+streetmodel.id+"/"+streetmodel.streetStatus).catch(x=> this.dataService.handleError(x));
    }

    bylcda(lcdaId: string)
    {
        return this.dataService.get('street/bylcda/'+lcdaId).catch(x=> this.dataService.handleError(x));
    }
    
}