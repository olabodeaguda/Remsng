import { Injectable } from '@angular/core';
import { DataService } from '../../shared/services/data.service';

@Injectable()
export class ReportService {

    constructor(private dateService: DataService) {
    }

    downloadReport(startDate: string, endDate: string) {
        return this.dateService.getBlob('revenue/' + startDate + '/' + endDate)
        .catch(error => this.dateService.handleError(error));
    }
}
