import { Component } from '@angular/core';
import {IMyDpOptions} from 'mydatepicker';

@Component({
    selector: 'app-report',
    templateUrl: '../views/report.component.html'
})

export class ReportComponent {

    isLoading: boolean = false;
    public myDatePickerOptions: IMyDpOptions = {
        dateFormat: 'dd-mm-yyyy',
    };

    constructor() {
    }

}
