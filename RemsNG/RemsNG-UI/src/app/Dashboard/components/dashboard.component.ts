import { Component, OnInit, Output, AfterViewInit } from '@angular/core';
import { HeaderComponent } from '../../shared/components/header.component';
import { StorageService } from '../../shared/services/storage.service';
import { UserModel } from '../../shared/models/user.model';
declare var jQuery: any;

@Component({
    selector: 'app-dsh',
    templateUrl: '../views/dashboard.component.html'
})

export class DashboardComponent implements OnInit {

    constructor() {
    }

    ngOnInit() {
        this.loadScript('../assets/dist/js/adminlte.min.js');
    }

    public loadScript(url) {
        console.log('preparing to load...');
        const node = document.createElement('script');
        node.src = url;
        node.type = 'text/javascript';
        document.getElementsByTagName('head')[0].appendChild(node);
     }

}
