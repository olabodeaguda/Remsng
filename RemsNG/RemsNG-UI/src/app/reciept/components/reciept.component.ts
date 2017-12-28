import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { PageModel } from '../../shared/models/page.model';
import { RecieptService } from '../services/reciept.service';
import { ToasterService } from 'angular2-toaster';
import { ReceiptModel } from '../models/receipt.model';
declare var jQuery: any;

@Component({
    selector: 'app-receipt',
    templateUrl: '../views/reciept.component.html'
})

export class RecieptComponent implements OnInit {

    receiptLst = [];
    pageModel: PageModel = new PageModel();
    isLoading: boolean = false;
    selectedReceipt: ReceiptModel = new ReceiptModel();
    @ViewChild('changestatus') approveReceiptModal: ElementRef;

    constructor(private receiptService: RecieptService,
        private toasterService: ToasterService) {

    }

    ngOnInit(): void {
        this.getByLcda();
    }

    getByLcda() {
        this.isLoading = true;
        this.receiptService.byLcda(this.pageModel)
            .subscribe(response => {
                const objschema = { data: [], totalPageCount: 0 };
                const res = Object.assign(objschema, response);
                this.receiptLst = res.data;
                this.pageModel.totalPageCount = res.totalPageCount;
                this.isLoading = false;
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    open(newStatus: string, data: any) {
        this.selectedReceipt = data;
        this.selectedReceipt.newPaymentStatus = newStatus;
        jQuery(this.approveReceiptModal.nativeElement).modal('show');
    }
    
    ReceiptAction(){

    }

}