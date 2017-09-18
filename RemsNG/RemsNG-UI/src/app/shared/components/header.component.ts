import { Component, Output } from '@angular/core';
import { UserModel } from '../models/user.model';
import { StorageService } from '../services/storage.service';

@Component({
    selector: 'app-hd',
    templateUrl: '../views/header.component.html'
})

export class HeaderComponent {

    userModel: UserModel;
    constructor(private storageService: StorageService) {
        this.userModel = storageService.get();
        storageService.usermodelEmit.subscribe((x) => {
            this.userModel = x;
        });
    }

    logout() {
        this.storageService.remove();
    }
}
