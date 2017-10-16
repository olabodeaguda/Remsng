import {Component, OnInit} from '@angular/core';
import { UserModel } from '../models/user.model';
import { StorageService } from '../services/storage.service';
declare var $: any;

@Component({
    selector: 'app-sbar',
    templateUrl: '../views/sideBar.component.html'
})

export class SideBarComponent implements OnInit {
    userModel: UserModel;
    constructor(private storageService: StorageService) {
        this.userModel = storageService.get();
        if(this.userModel === null){            
        this.userModel = new UserModel();
        }
        storageService.usermodelEmit.subscribe((x) => {
            this.userModel = x;
        });
    }

    ngOnInit() {
    }
}
