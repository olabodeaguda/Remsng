import { Component, OnInit } from '@angular/core';
import { IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { ToasterService } from 'angular2-toaster';
import { ReportService } from '../services/report.service';
import * as FileSaver from 'file-saver';
import { AppSettings } from '../../shared/models/app.settings';
import { isNullOrUndefined } from 'util';
import { CategoryService } from '../../Category/services/category.service';

@Component({
    selector: 'app-report',
    templateUrl: '../views/report.component.html'
})

export class ReportComponent implements OnInit {

    startDate: string = '';
    endDate: string = '';
    htmlresult: string = '';
    yrLst = [];
    selectedYr;
    selectCategory;
    isLoading: boolean = false;
    catgories = [];
    public myDatePickerOptions: IMyDpOptions = {
        dateFormat: 'dd-mm-yyyy',
    };

    constructor(private toasterService: ToasterService,
        private reportService: ReportService,
        private appsettings: AppSettings, private catService: CategoryService ) {
    }

    ngOnInit() {
        this.yrLst = this.appsettings.getYearList();
        this.getCategoryType();
    }

    getCategoryType(){
        this.catService.getCategoryType()
        .subscribe(response=>{
            this.catgories = response.data;
        }, error=>{
            console.log(error)
        })
    }

    onDateChanged(event: IMyDateModel, dataType: string) {
        if (dataType === 'startDate') {
            this.startDate = event.formatted;
        } else if (dataType === 'endDate') {
            this.endDate = event.formatted;
        }
        console.log(this.startDate.length);
    }

    download() {
        if (this.startDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.endDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.startDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is in the wrong format');
            return;
        } else if (this.endDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        }

        this.isLoading = true;
        this.reportService.downloadReport(this.startDate, this.endDate)
            .subscribe(response => {
                this.isLoading = false;
                FileSaver.saveAs(response, this.startDate + '-' + this.endDate + 'report' + '.xlsx');
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    downloadReportBreakDown() {
        if (this.startDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.endDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.startDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is in the wrong format');
            return;
        } else if (this.endDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        }

        this.isLoading = true;
        this.reportService.downloadReportBreakDown(this.startDate, this.endDate)
            .subscribe(response => {
                this.isLoading = false;
                FileSaver.saveAs(response, this.startDate + '-' + this.endDate + 'reportbreakDown' + '.xlsx');
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    downloadReportBreakDownSeperate() {
        if (this.startDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.endDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.startDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is in the wrong format');
            return;
        } else if (this.endDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        }

        this.isLoading = true;
        this.reportService.downloadReportBreakDownSeperate(this.startDate, this.endDate)
            .subscribe(response => {
                this.isLoading = false;
               // let downloadUrl: string = `/remsng/quarterlyreport/${response.data}`;
               // window.open(downloadUrl);

                FileSaver.saveAs(response, this.startDate + '-' + this.endDate + 'reportbreakDownseperate' + '.xlsx');
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    search() {
        if (this.startDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.endDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.startDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is in the wrong format');
            return;
        } else if (this.endDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        }

        this.isLoading = true;
        this.reportService.html(this.startDate, this.endDate)
            .subscribe(response => {
                this.isLoading = false;
                if (response.code === '00') {
                    this.htmlresult = response.description;
                } else {
                    this.htmlresult = '';
                }
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    searchReportBreakDown() {
        if (this.startDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.endDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.startDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is in the wrong format');
            return;
        } else if (this.endDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        }

        this.isLoading = true;
        this.reportService.downloadReportBreakDownhtml(this.startDate, this.endDate)
            .subscribe(response => {
                this.isLoading = false;
                if (response.code === '00') {
                    this.htmlresult = response.description;
                } else {
                    this.htmlresult = '';
                }
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    downloadTWithoutDN() {
        if (isNullOrUndefined(this.selectedYr)) {
            this.toasterService.pop('error', 'Error', 'Billing year is required');
            return;
        }
        this.isLoading = true;
        this.reportService.downloadReportTaxpayerWithOutDN(this.selectedYr)
            .subscribe(response => {
                this.isLoading = false;
                 FileSaver.saveAs(response, this.selectedYr + 'report' + '.xlsx');
                // const blob = new Blob([response], { type: 'text/xlsx' });
                // const url = window.URL.createObjectURL(blob);
                // window.open(url);
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    downloadByWard() {
        if (this.startDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.endDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.startDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is in the wrong format');
            return;
        } else if (this.endDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        }
        
        this.isLoading = true;
        this.reportService.reportByWard(this.startDate, this.endDate)
            .subscribe(response => {
                this.isLoading = false;
                FileSaver.saveAs(response, this.selectedYr + 'reportbyward' + '.xlsx');
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    downloadByCategory() {
        if (this.startDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.endDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.startDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is in the wrong format');
            return;
        } else if (this.endDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        }
        console.log(this.selectCategory);

        this.isLoading = true;
        this.reportService.downloadReportByCategory(this.startDate, this.endDate, this.selectCategory)
            .subscribe(response => {
                this.isLoading = false;
                FileSaver.saveAs(response, this.startDate + '-' + this.endDate + 'report' + '.xlsx');
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    downloadByCategoryDetails() {
        if (this.startDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.endDate.length < 1) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        } else if (this.startDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is in the wrong format');
            return;
        } else if (this.endDate.length < 10) {
            this.toasterService.pop('error', 'Error', 'Start Date is required');
            return;
        }

        this.isLoading = true;
        this.reportService.downloadReportByCategoryExt(this.startDate, this.endDate, this.selectCategory)
            .subscribe(response => {
                this.isLoading = false;
                FileSaver.saveAs(response, this.startDate + '-' + this.endDate + 'report' + '.xlsx');
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

}
