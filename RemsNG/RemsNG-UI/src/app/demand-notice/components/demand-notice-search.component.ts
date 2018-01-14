import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { DemandNoticeSearch } from "../models/demand-notice.search";
import { AppSettings } from "../../shared/models/app.settings";
import { PageModel } from "../../shared/models/page.model";
import { ToasterService } from "angular2-toaster";
import { DemandNoticeTaxpayerService } from "../services/demand-noticeTaxpayer.service";
import * as FileSaver from 'file-saver'
import { AmountDueModel } from "../models/amount-due.model";
import { DemandNoticePaymentModel } from "../models/demandNotice-payment.model";
import { BankService } from "../../shared/services/bank.service";
import { DemandNoticePaymentService } from "../services/demand-notice-payment.service";
declare var jQuery: any;

@Component({
    selector: 'app-taxpayerSerch',
    templateUrl: '../views/demand-notice-search.component.html'
})

export class DemandNoticeSearchComponent implements OnInit {
    paymentList = [];
    isLoading: boolean;
    taxpayersLst = [];
    banks = [];
    pageModel: PageModel = new PageModel();
    amountDueList = [];
    searchModel: DemandNoticeSearch = new DemandNoticeSearch();
    amountDueModel: AmountDueModel = new AmountDueModel();
    @ViewChild('addModal') addModel: ElementRef;
    @ViewChild('paymentModal') paymentModal: ElementRef;
    @ViewChild('paymentListModal') paymentListModal: ElementRef;
    dnpModel: DemandNoticePaymentModel = new DemandNoticePaymentModel();
    isLoadingMini: boolean = false;
    isLoadingPayment: boolean = false;
    isLoadingReceipt: boolean = false;
    constructor(private appsettings: AppSettings,
        private toasterService: ToasterService,
        private dnTaxpayer: DemandNoticeTaxpayerService,
        private bankService: BankService,
        private dnPaymentService: DemandNoticePaymentService) {
    }

    ngOnInit(): void {
        this.getBanks();
    }

    downloadDN(url: string) {
        this.isLoading = true;
        this.dnTaxpayer.downloadRpt(url).map(response => {
            this.isLoading = false;
            let blob = response;
            FileSaver.saveAs(blob, url + '.pdf');
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Download Error', error);
        }).subscribe();
    }

    getBanks() {
        this.bankService.getBanks().subscribe(response => {
            this.banks = response;
        }, error => {
            this.toasterService.pop('error', 'Error', error);
        })
    }

    /*downloadReciept(id: string, billingNumber: string) {
        this.isLoadingReceipt = true;
        this.dnTaxpayer.downloadReceipt(id).map(response => {
            this.isLoadingReceipt = false;
            let blob = response;
            FileSaver.saveAs(blob, billingNumber + 'Receipt' + '.pdf');
        }, error => {
            alert('error');
            this.isLoadingReceipt = false;
            this.toasterService.pop('error', 'Download Error', error);
        }).subscribe();
    }*/

    downloadReciept(id: string, billingNumber: string) {
        this.isLoadingReceipt = true;
        this.dnTaxpayer.downloadReceipt(id)
            .subscribe(response => {
                this.isLoadingReceipt = false;
                let blob = response;
                FileSaver.saveAs(blob, billingNumber + 'Receipt' + '.pdf');
            }, error => {
                this.isLoadingReceipt = false;
                this.toasterService.pop('error', 'Download Error', error);
            });
    }

    openPayment(billingno: string) {
        this.dnpModel.billingNumber = billingno;
        jQuery(this.paymentModal.nativeElement).modal('show');
    }

    openReciept(billingno: string) {
        this.dnpModel.billingNumber = billingno;
        if (this.dnpModel.billingNumber.length < 1) {
            return;
        }
        this.getListPayment(billingno);
        jQuery(this.paymentListModal.nativeElement).modal('show');
    }

    getListPayment(billingno: string) {
        this.isLoadingReceipt = true;
        this.dnPaymentService.getRecipetList(billingno)
            .subscribe(response => {
                this.isLoadingReceipt = false;
                this.paymentList = response;
            }, error => {
                this.isLoadingReceipt = false;
                this.toasterService.pop('error', 'Error', error);
            })
    }

    registerpayment() {
        if (this.dnpModel.amount < 0) {
            this.toasterService.pop('error', 'Error', 'Amount paid is required');
            return;
        } else if (this.dnpModel.bankId.length < 1) {
            this.toasterService.pop('error', 'Error', 'Bank is required');
            return;
        } else if (this.dnpModel.referenceNumber.length < 1) {
            this.toasterService.pop('error', 'Error', 'Reference number is required');
            return;
        }
        this.isLoadingMini = true;
        this.dnPaymentService.registerPayment(this.dnpModel)
            .subscribe(response => {
                if (response.code === '00') {
                    this.toasterService.pop('success', response.description);
                    this.isLoadingMini = false;
                    jQuery(this.paymentModal.nativeElement).modal('hide');
                    this.dnpModel = new DemandNoticePaymentModel();
                } else {
                    this.toasterService.pop('error', 'Error!!!', response.description);
                }
            }, error => {
                this.isLoadingMini = false;
                this.toasterService.pop('error', 'Error!!!', error);
            })

    }

    search() {
        if (this.searchModel.billingNo.length < 1) {
            this.toasterService.pop('warning', 'Warning', 'Billing number is required');
            return;
        }

        this.dnTaxpayer.ByBillingno(this.searchModel.billingNo)
            .subscribe(response => {
                if (response.code == '00') {
                    this.taxpayersLst = response.data;
                }
                if (this.taxpayersLst.length < 1) {
                    this.toasterService.pop('warning', 'Empty!!!', 'no record found');
                }
            }, error => {
                this.toasterService.pop('error', 'error', error);
            })
    }

    openEdit(billingNo: string) {
        this.amountDueModel.billingNumber = billingNo
        jQuery(this.addModel.nativeElement).modal('show');
        this.getAmountDueByBillingNo(billingNo);
    }

    getAmountDueByBillingNo(billingNo: string) {
        this.isLoading = true;
        this.dnTaxpayer.amountDueByBillingNo(billingNo).subscribe(response => {
            this.amountDueList = response;
            this.isLoading = false;
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    selectItems(data) {
        let s = this.amountDueModel.billingNumber;
        this.amountDueModel = data;
        this.amountDueModel.billingNumber = s;
    }

    getSum(ty: string): number {
        let sum = 0;
        for (let i = 0; i < this.amountDueList.length; i++) {
            if (ty === 'itemAmount') {
                sum += Number.parseFloat(this.amountDueList[i].itemAmount);
            } else if (ty === 'amountPaid') {
                sum += Number.parseFloat(this.amountDueList[i].amountPaid);
            }
        }
        return sum;
    }

    updateValue() {
        if (this.amountDueModel.id.length < 1) {
            return;
        }
        this.amountDueModel.isLoading = true;
        this.dnTaxpayer.UpdateAmount(this.amountDueModel)
            .subscribe(response => {
                this.amountDueModel.isLoading = false;
                if (response.code === '00') {
                    this.getAmountDueByBillingNo(this.amountDueModel.billingNumber);
                    let s: string = this.amountDueModel.billingNumber;
                    this.amountDueModel = new AmountDueModel();
                    this.amountDueModel.billingNumber = s;
                }
            }, error => {
                this.amountDueModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })
    }
}