import { Component } from '@angular/core';
import { IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { ToasterService } from 'angular2-toaster';
import { ReportService } from '../services/report.service';
import * as FileSaver from 'file-saver';

@Component({
    selector: 'app-report',
    templateUrl: '../views/report.component.html'
})

export class ReportComponent {

    startDate: string = '';
    endDate: string = '';

    isLoading: boolean = false;
    public myDatePickerOptions: IMyDpOptions = {
        dateFormat: 'dd-mm-yyyy',
    };

    constructor(private toasterService: ToasterService,
        private reportService: ReportService) {
    }

    onDateChanged(event: IMyDateModel, dataType: string) {
        if (dataType === 'startDate') {
            this.startDate = event.formatted;
        } else if (dataType === 'endDate') {
            this.endDate = event.formatted;
        }
        console.log(this.startDate.length);
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

        this.reportService.downloadReport(this.startDate, this.endDate)
        .subscribe(response => {
                FileSaver.saveAs(response, this.startDate + '-' + this.endDate + 'report' + '.xlsx');
        }, error => {
            this.toasterService.pop('error', 'Error', error);
        });
    }


}
