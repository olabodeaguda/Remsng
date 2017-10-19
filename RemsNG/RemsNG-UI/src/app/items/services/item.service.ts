import {Injectable} from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { PageModel } from "../../shared/models/page.model";
import { ItemModel } from "../models/item.model";

@Injectable()
export class ItemService{

    constructor(private dataservice:DataService) {
    }

    getByLcdaId(lcdaId:string, pageModel: PageModel){
        this.dataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        this.dataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        return this.dataservice.get('item/bylcdapaginated/'+lcdaId).catch(x=> this.dataservice.handleError(x));
    }

    add(itemmodel:ItemModel){
        return this.dataservice.post('item',{
            itemDescription: itemmodel.itemDescription,
            lcdaId: itemmodel.lcdaId
        }).catch(x=> this.dataservice.handleError(x));
    }

    update(itemmodel: ItemModel){
        return this.dataservice.put('item/'+itemmodel.id,{
            id:itemmodel.id,
            itemDescription: itemmodel.itemDescription,
            lcdaId: itemmodel.lcdaId
        })
    }

    changeStatus(itemmodel: ItemModel){
        return this.dataservice.post('item/changestatus',{
            id: itemmodel.id,
            itemStatus: itemmodel.itemStatus
        }).catch(x=> this.dataservice.handleError(x));
    }

}