import { Component, OnInit, Output, AfterViewInit, OnChanges, SimpleChanges, ViewChild, ElementRef } from '@angular/core';
import { HeaderComponent } from '../../shared/components/header.component';
import { StorageService } from '../../shared/services/storage.service';
import { UserModel } from '../../shared/models/user.model';
import { ReportService } from '../../report/services/report.service';
import { ToasterService } from 'angular2-toaster';
import { WardService } from '../../ward/services/ward.service';
declare var jQuery: any;
import { BaseChartDirective } from 'ng2-charts/ng2-charts';

@Component({
    selector: 'app-dsh',
    templateUrl: '../views/dashboard.component.html'
})

export class DashboardComponent implements OnInit {
    @ViewChild('baseChart') public chart2: BaseChartDirective;
    @ViewChild('baseChart1') public chart3: BaseChartDirective;

    public dataSourceReceivables = {
        'data': [{ data: [0], label: '' }],
        'barChartLegend': true,
        'barChartOptions': {
            scaleShowVerticalLines: false,
            responsive: true
        },
        'barChartType': 'bar',
        'labels': []
    };

    dataSourceRevenue = {
        'data': [{ data: [0], label: '' }],
        'barChartLegend': true,
        'barChartOptions': {
            scaleShowVerticalLines: false,
            responsive: true
        },
        'barChartType': 'bar',
        'labels': []
    };

    constructor(private reportService: ReportService,
        private toasterService: ToasterService, private wardservice: WardService) {
    }

    ngOnInit() {
        this.getLabel();
        this.getReceivables();
    }

    getLabel() {
        this.wardservice.all().subscribe(response => {
            this.dataSourceReceivables.labels = response.map(({ wardName }) => wardName);
        }, error => {
            this.toasterService.pop('warning', 'Warning', error);
        });
    }

    public getReceivables() {
        this.reportService.graphRecievables()
            .subscribe(response => {
                this.chart2.datasets = response.map(function (data, index) {
                    const v: Array<any> = new Array();
                    v.push(data.receivables);
                    return { data: v, label: data.label };
                });

                this.chart3.datasets = response.map(function (data, index) {
                    const s: Array<any> = new Array();
                    s.push(data.amountPaid);
                    return { data: s, label: data.label };
                });

                // this.chart2.datasets = this.dataSourceReceivables.data;
                //  this.chart3.data = this.dataSourceRevenue.data;

                this.chart2.ngOnChanges({} as SimpleChanges);
                this.chart3.ngOnChanges({} as SimpleChanges);
            }, error => {
                this.toasterService.pop('error', 'Error', error);
            });
    }
}
