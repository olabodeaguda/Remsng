import {Injectable} from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { AddressModel } from "../models/address.model";

@Injectable()
export class AddressService{

    constructor(private dataservice: DataService) {        
    }

    byOwnerId(ownderId:string, lcdaId: string){
        return this.dataservice.get('address/byownerid/'+ownderId+'/'+lcdaId).catch(x=> this.dataservice.handleError(x));
    }

    add(addressmodel: AddressModel){
        return this.dataservice.post('address',{
            addressnumber: addressmodel.addressnumber,
            streetId: addressmodel.streetId,
            ownerId: addressmodel.ownerId,
            lcdaid: addressmodel.lcdaid
        }).catch(x=> this.dataservice.handleError(x));
    }

    update(addressmodel: AddressModel){
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