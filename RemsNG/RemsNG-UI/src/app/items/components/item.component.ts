import {Component, OnInit} from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector:'app-item',
    templateUrl:'../views/item.component.html'
})

export class ItemComponent implements OnInit{

    constructor(private activeRoute: ActivatedRoute) {        
    }

    ngOnInit(): void {
        this.initializePage();
    }

    initializePage() {
        this.activeRoute.params.subscribe((param: any) => {
            this.getItems(param["id"]);
        });
    }

    getItems(lcdaId: string){

    }
}