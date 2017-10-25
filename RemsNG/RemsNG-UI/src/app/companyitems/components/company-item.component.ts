import { Component, OnInit } from '@angular/core';
import { ComponentItemService } from "../services/company-item.service";

@Component({
    selector: 'company-item',
    templateUrl: '../views/company-item.component.html'
})

export class ComponentItemComponent implements OnInit {

    companyLst = [];

    constructor(private companyitemservice: ComponentItemService) {
    }

    ngOnInit(): void {

    }
}