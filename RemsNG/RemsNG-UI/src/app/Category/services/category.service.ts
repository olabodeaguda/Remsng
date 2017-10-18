import { Injectable } from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { CategoryModel } from "../models/category.model";

@Injectable()
export class CategoryService {

    constructor(private dataService: DataService) {
    }

    getAll(lcdaId: string) {
        return this.dataService.get('taxpayercategory/bylcda/' + lcdaId).catch(x => this.dataService.handleError(x));
    }

    add(categoryModel: CategoryModel) {
        return this.dataService.post('taxpayercategory', {
            taxpayerCategoryName: categoryModel.taxpayerCategoryName,
            lcdaId: categoryModel.lcdaId
        }).catch(x => this.dataService.handleError(x));
    }

    update(categoryModel: CategoryModel){
        return this.dataService.put('taxpayercategory', {
            taxpayerCategoryName: categoryModel.taxpayerCategoryName,
            lcdaId: categoryModel.lcdaId,
            id: categoryModel.id
        }).catch(x => this.dataService.handleError(x));
    }

    remove(catId: string){
        return this.dataService.delete('taxpayercategory/'+catId).catch(x=> this.dataService.handleError(x));
    }

}