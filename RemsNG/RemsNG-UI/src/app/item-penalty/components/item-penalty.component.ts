import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { PageModel } from "../../shared/models/page.model";
import { ItemPenaltyModel } from "../models/item-penalty.model";
import { ItemModel } from "../../items/models/item.model";
import { ActivatedRoute } from '@angular/router';
import { ItemService } from "../../items/services/item.service";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { ItemPenaltyService } from "../services/item-penalty.service";
declare var jQuery: any;

@Component({
    selector: 'app.item-penalty',
    templateUrl: '../views/item-penalty.component.html'
})

export class ItemPenaltyComponent implements OnInit {

    pageModel: PageModel;
    itempModel: ItemPenaltyModel;
    itemPs = [];
    item: ItemModel//
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changeModal') changeModal: ElementRef;
    isLoading: boolean = false;

    constructor(private activeRoute: ActivatedRoute, private itemService: ItemService,
        private toasterService: ToasterService, private itempservice: ItemPenaltyService) {
        this.pageModel = new PageModel();
        this.itempModel = new ItemPenaltyModel();
        this.item = new ItemModel();
    }

    ngOnInit(): void {
        this.activeRoute.params.subscribe((param: any) => {
            this.getItem(param["id"]);
        });
    }

    getItem(itemId: string) {
        this.isLoading = true;
        this.itemService.byId(itemId).subscribe(response => {
            this.isLoading = false;
            const result = Object.assign(new ItemModel(), response.json());
            this.item = result;
            this.getitemspenalty();
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    getitemspenalty() {
        if (this.item.id.length < 1) {
            this.toasterService.pop('error', 'Error', 'An error occur. Please refresh your page else contact an administrator if problem persist');
            return;
        }

        this.itempservice.getByitemIdPaginated(this.item.id, this.pageModel)
            .subscribe(response => {
                const objschema = { data: [], totalPageCount: 1 };
                const result = Object.assign(objschema, response.json());
                this.itemPs = result.data;
                this.pageModel.totalPageCount = result.totalPageCount
            }, error => {

            })
    }

    open(eventType: string, data: any) {
        if (eventType === 'ADD' || eventType === 'EDIT') {
            if (eventType === 'ADD') {
                this.itempModel = new ItemPenaltyModel();
            } else {
                this.itempModel = data;
            }
            this.itempModel.itemId = this.item.id
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === 'CHANGE_STATUS') {
            this.itempModel = data;
            jQuery(this.changeModal.nativeElement).modal('show');
        }
        this.itempModel.eventType = eventType;
    }
    actions() {
        if (this.itempModel.itemId.length < 1) {
            this.toasterService.pop('error', 'Error', 'An error occur. Please refresh your page else contact an administrator if problem persist');
            return;
        } else if (this.itempModel.amount < 1) {
            this.toasterService.pop('error', 'Error', "invalid amount. It can't be less that zero");
            return;
        } else if (this.itempModel.duration.length < 1) {
            this.toasterService.pop('error', 'Error', "Duration is required");
            return;
        }

        this.itempModel.isLoading = true;
        if (this.itempModel.eventType === 'ADD') {
            this.itempservice.add(this.itempModel).subscribe(response => {
                this.itempModel.isLoading = false;
                const resp = Object.assign(new ResponseModel(), response.json());
                if (resp.code) {
                    this.toasterService.pop('success', 'Success', resp.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getitemspenalty();
                } else {
                    this.toasterService.pop('error', 'Error', resp.description);
                }
            }, error => {
                this.getitemspenalty();
                jQuery(this.addModal.nativeElement).modal('hide');
                this.toasterService.pop('error', 'Error', error);
            })
        } else if (this.itempModel.eventType === 'EDIT') {
            this.itempservice.edit(this.itempModel).subscribe(response => {
                this.itempModel.isLoading = false;
                const resp = Object.assign(new ResponseModel(), response.json());
                if (resp.code) {
                    this.toasterService.pop('success', 'Success', resp.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getitemspenalty();
                } else {
                    this.toasterService.pop('error', 'Error', resp.description);
                }
            }, error => {
                this.getitemspenalty();
                jQuery(this.addModal.nativeElement).modal('hide');
                this.toasterService.pop('error', 'Error', error);
            })
        } else if (this.itempModel.eventType === 'CHANGE_STATUS') {            
            if(this.itempModel.currentstatus == undefined){
                this.itempModel.isLoading = false;
                this.toasterService.pop('error', 'Error', 'penalty status is required!!!');
                return;
            }
            if (this.itempModel.currentstatus === this.itempModel.penaltyStatus) {
                this.itempModel.isLoading = false;
                this.toasterService.pop('warning', 'Warning', 'No changes to penalty status');
                return;
            }
            this.itempservice.changeStatus(this.itempModel).subscribe(response => {
                this.itempModel.isLoading = false;
                const resp = Object.assign(new ResponseModel(), response.json());
                if (resp.code) {
                    this.toasterService.pop('success', 'Success', resp.description);
                    jQuery(this.changeModal.nativeElement).modal('hide');
                    this.getitemspenalty();
                } else {
                    this.toasterService.pop('error', 'Error', resp.description);
                }
            }, error => {
                this.itempModel.isLoading = false;
                jQuery(this.changeModal.nativeElement).modal('hide');
                this.toasterService.pop('error', 'Error', error);
            })
        }

    }

    next() {
        if (this.pageModel.pageNum > 1 && this.itemPs.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getitemspenalty();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getitemspenalty();
    }

}