import {Injectable} from '@angular/core';
import { DataService } from '../../shared/services/data.service';
import { AddressGlobalModel } from '../models/addressGlobal.model';

@Injectable()
export class AddressGlobalService{
    constructor(private dataservice: DataService) {        
    }

    byOwnerId(ownderId:string, lcdaId: string){
        return this.dataservice.get('address/byownerid/'+ownderId+'/'+lcdaId).catch(x=> this.dataservice.handleError(x));
    }

    add(addressmodel: AddressGlobalModel){
        return this.dataservice.post('address',{
            addressnumber: addressmodel.addressnumber,
            streetId: addressmodel.streetId,
            ownerId: addressmodel.ownerId,
            lcdaid: addressmodel.lcdaid
        }).catch(x=> this.dataservice.handleError(x));
    }

    update(addressmodel: AddressGlobalModel){
        return this.dataservice.put('address',{
            addressnumber: addressmodel.addressnumber,
            streetId: addressmodel.streetId,
            ownerId: addressmodel.ownerId,
            lcdaid: addressmodel.lcdaid,
            id: addressmodel.id
        }).catch(x=> this.dataservice.handleError(x));
    }

    remove(id: string){
        return this.dataservice.delete('address/'+id).catch(x=> this.dataservice.handleError(x));
    }
}