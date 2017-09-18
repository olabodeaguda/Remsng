import { Component, OnInit } from '@angular/core';
import { PageModel } from '../../shared/models/page.model';
import { LcdaModel } from '../models/lcda.models';
import { LcdaService } from '../services/lcda.services';

@Component({
    selector: 'app-lcda',
    templateUrl: '../views/lcda.component.html'
})

export class LcdaComponent implements OnInit {

    pageModel: PageModel;
    lcdaModel: LcdaModel;
    isLoading: boolean = false;

    constructor(private lcdaService: LcdaService) {
        this.pageModel = new PageModel();
        this.lcdaModel = new LcdaModel();
    }

    ngOnInit() {
        this.getLcda();
    }

    getLcda() {
        this.isLoading = true;


    }

}
