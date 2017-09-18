import {Component} from '@angular/core';
import { UserModel } from '../models/user.model';
import { StorageService } from '../services/storage.service';

@Component({
    selector: 'app-sbar',
    templateUrl: '../views/sideBar.component.html'
})

export class SideBarComponent {
    userModel: UserModel;
    constructor(private storageService: StorageService) {
        this.userModel = storageService.get();
        storageService.usermodelEmit.subscribe((x) => {
            this.userModel = x;
        });
    }
}
