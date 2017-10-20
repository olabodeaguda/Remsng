import { Injectable } from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { PageModel } from "../../shared/models/page.model";
import { ItemPenaltyModel } from "../models/item-penalty.model";

@Injectable()
export class ItemPenaltyService {

    constructor(private dataservice: DataService) {
    }

    getByitemId(itemId: string) {
        return this.dataservice.get('itempenalty/byitem/' + itemId)
            .catch(x => this.dataservice.handleError(x));
    }

    getByitemIdPaginated(itemId: string, pageModel: PageModel) {
        this.dataservice.addToHeader('pageNum', pageModel.pageNum.toString());
        this.dataservice.addToHeader('pageSize', pageModel.pageSize.toString());
        return this.dataservice.get('itempenalty/byitempaginated/' + itemId)
            .catch(x => this.dataservice.handleError(x));
    }

    add(itempModel: ItemPenaltyModel) {
        return this.dataservice.post('itempenalty', {
            itemId: itempModel.itemId,
            duration: itempModel.duration,
            isPercentage: itempModel.isPercentage,
            amount: itempModel.amount
        }).catch(x => this.dataservice.handleError(x));
    }

    edit(itempModel: ItemPenaltyModel) {
        return this.dataservice.put('itempenalty', {
            itemId: itempModel.itemId,
            duration: itempModel.duration,
            isPercentage: itempModel.isPercentage,
            amount: itempModel.amount,
            id: itempModel.id
        }).catch(x => this.dataservice.handleError(x));
    }

    changeStatus(itempModel: ItemPenaltyModel) {
        return this.dataservice.post('itempenalty/changestatus', {
            penaltyStatus: itempModel.currentstatus,
            id: itempModel.id
        }).catch(x => this.dataservice.handleError(x));
    }

}