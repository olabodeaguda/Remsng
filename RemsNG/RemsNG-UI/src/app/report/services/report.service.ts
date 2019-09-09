import { Injectable } from '@angular/core';
import { DataService } from '../../shared/services/data.service';

@Injectable()
export class ReportService {

    constructor(private dateService: DataService) {
    }

    downloadReport(startDate: string, endDate: string) {
        return this.dateService.getBlob('report/revenue/' + startDate + '/' + endDate)
        .catch(error => this.dateService.handleError(error));
    }

// reportdata
    html(startDate: string, endDate: string) {
        return this.dateService.get('report/revenuehtml/' + startDate + '/' + endDate)
        .catch(error => this.dateService.handleError(error));
    }

    downloadReportBreakDown(startDate: string, endDate: string) {
        return this.dateService.getBlob('report/outstandingbybillno/' + startDate + '/' + endDate)
        .catch(error => this.dateService.handleError(error));
    }

    downloadReportByCategory(startDate: string, endDate: string) {
        return this.dateService.getBlob('report/category/' + startDate + '/' + endDate)
        .catch(error => this.dateService.handleError(error));
    }

    downloadReportBreakDownhtml(startDate: string, endDate: string) {
        return this.dateService.get('report/outstandingbybillnohtml/' + startDate + '/' + endDate)
        .catch(error => this.dateService.handleError(error));
    }

    downloadReportBreakDownSeperate(startDate: string, endDate: string) {
        return this.dateService.get('report/outstandingbybillnoseperate/' + startDate + '/' + endDate)
        .catch(error => this.dateService.handleError(error));
    }


    graphRecievables() {
        return this.dateService.get('report/reportreceivables')
        .catch(error => this.dateService.handleError(error));
    }

    graphRevenue() {
        return this.dateService.get('report/reportrevenue')
        .catch(error => this.dateService.handleError(error));
    }

    
    downloadReportTaxpayerWithOutDN(billingYr: number) {
        return this.dateService.getBlob(`report/withoutdn/${billingYr}`)
        .catch(error => this.dateService.handleError(error));
    }

    reportByWard(startDate: string, endDate: string) {
        return this.dateService.getBlob('report/byward/' + startDate + '/' + endDate)
            .catch(error => this.dateService.handleError(error));
    }

}

