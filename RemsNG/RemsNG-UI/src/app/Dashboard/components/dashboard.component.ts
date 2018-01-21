import { Component, OnInit, Output, AfterViewInit } from '@angular/core';
import { HeaderComponent } from '../../shared/components/header.component';
import { StorageService } from '../../shared/services/storage.service';
import { UserModel } from '../../shared/models/user.model';
import { ReportService } from '../../report/services/report.service';
import { ToasterService } from 'angular2-toaster';
declare var jQuery: any;

@Component({
    selector: 'app-dsh',
    templateUrl: '../views/dashboard.component.html'
})

export class DashboardComponent implements OnInit {

    width = 600;
    height = 400;
    type = 'column2d';
    dataFormat = 'json';
    dataSourceReceivables = {
        'chart': {
            'caption': 'Demand Notice Receivables',
            'subCaption': 'All Receivables by Wards',
            'numberPrefix': 'N',
            'theme': 'ocean',
            'xAxisName': 'Ward',
            'yAxisName': 'Receivables (In Naira)'
        },
        'data': []
    };

    dataSourceRevenue = {
        'chart': {
            'caption': 'Demand Notice Revenue',
            'subCaption': 'Revenue by Wards for the year',
            'numberPrefix': 'N',
            'theme': 'ocean',
            'xAxisName': 'Ward',
            'yAxisName': 'Revenues (In Naira)'
        },
        'data': []
    };

    constructor(private reportService: ReportService,
        private toasterService: ToasterService) {
    }

    ngOnInit() {
       // this.loadScript('assets/dist/js/adminlte.min.js');
       this.getReceivables();
       this.getRevenue();
    }

    public getReceivables() {
        this.reportService.graphRecievables()
        .subscribe(response => {
            this.dataSourceReceivables.data = response;
        }, error => {
            this.toasterService.pop('error', 'Error', error);
        });
     }

     public getRevenue() {
        this.reportService.graphRevenue()
        .subscribe(response => {
            this.dataSourceRevenue.data = response;
        }, error => {
            this.toasterService.pop('error', 'Error', error);
        });
     }

}
