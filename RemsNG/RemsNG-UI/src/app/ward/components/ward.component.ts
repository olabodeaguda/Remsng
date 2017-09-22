import {Component} from '@angular/core';
import { PageModel } from '../../shared/models/page.model';
import { WardModel } from '../models/ward.model';

@Component({
    selector: 'app-ward',
    templateUrl: '../views/ward.component.html'
})
export class WardComponent {

    wardlst = [];
    pageModel: PageModel;
    wardModel: WardModel;

    constructor() {
        this.pageModel = new PageModel();
        this.wardModel = new WardModel();
    }



}

