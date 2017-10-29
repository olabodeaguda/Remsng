import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PageModel } from "../../shared/models/page.model";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { LcdaModel } from "../../lcda/models/lcda.models";
import { LcdaService } from "../../lcda/services/lcda.services";
import { ItemService } from "../services/item.service";
import { ItemModel } from "../models/item.model";
declare var jQuery: any;

@Component({
    selector: 'app-item',
    templateUrl: '../views/item.component.html'
})

export class ItemComponent implements OnInit {
    lcdaModel: LcdaModel;
    items = [];
    pageModel: PageModel;
    isLoading: boolean = false;
    itemmodel: ItemModel;
    currentstatus: string = '';

    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changeModal') changeModal: ElementRef;

    constructor(private activeRoute: ActivatedRoute, private toasterService: ToasterService,
        private lcdaService: LcdaService, private itemService: ItemService) {
        this.lcdaModel = new LcdaModel();
        this.pageModel = new PageModel();
        this.itemmodel = new ItemModel();
    }

    ngOnInit(): void {
        this.initializePage();
    }

    initializePage() {
        this.activeRoute.params.subscribe((param: any) => {
            this.getLcda(param["id"]);
        });
    }

    open(eventType: string, data: any) {
        if (eventType === 'ADD' || eventType === 'EDIT') {
            if (eventType === 'ADD') {
                this.itemmodel = new ItemModel();
            } else {
                this.itemmodel = data;
            }
            this.itemmodel.lcdaId = this.lcdaModel.id;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === 'CHANGE_STATUS') {
            this.itemmodel = data;
            this.currentstatus = this.itemmodel.itemStatus;
            jQuery(this.changeModal.nativeElement).modal('show');
        }
        this.itemmodel.eventType = eventType;
    }


    getLcda(lcdaId: string) {
        this.isLoading = true;
        this.lcdaService.getLCdaById(lcdaId).subscribe(response => {
            this.isLoading = false;
            const objSchema = Object.assign(new ResponseModel(), response);
            if (objSchema.code == '00') {
                this.lcdaModel = objSchema.data;
                this.getItems();
            }
            else {
                this.toasterService.pop('error', 'Error', objSchema.desciption);
            }

        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    actions() {
        if (this.itemmodel.itemDescription.length < 1) {
            this.toasterService.pop('error', 'Error', 'Item description is required');
            return;
        }

        this.itemmodel.isLoading = true;
        if (this.itemmodel.eventType === 'ADD') {
            this.itemService.add(this.itemmodel).subscribe(response => {
                this.itemmodel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code == '00') {
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getItems();
                } else {
                    this.getItems();
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.itemmodel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
        } else if (this.itemmodel.eventType === 'EDIT') {
            this.itemService.update(this.itemmodel).subscribe(response => {
                this.itemmodel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code == '00') {
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getItems();
                } else {
                    this.getItems();
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.itemmodel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
        } else if (this.itemmodel.eventType === 'CHANGE_STATUS') {
            if (this.itemmodel.id.length < 1) {
                this.toasterService.pop('error', 'Error', 'Please refresh you page and try again');
                this.itemmodel.isLoading = false;
                return;
            }
            else if (this.itemmodel.itemStatus.length < 1 || this.currentstatus.length < 1) {
                this.toasterService.pop('error', 'Error', 'Please select new status');
                this.itemmodel.isLoading = false;
                return;
            }
            
            this.itemmodel.itemStatus = this.currentstatus;
            this.itemService.changeStatus(this.itemmodel).subscribe(response => {
                this.itemmodel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code == '00') {
                    this.currentstatus = '';
                    jQuery(this.changeModal.nativeElement).modal('hide');
                    this.getItems();
                } else {
                    this.getItems();
                    this.toasterService.pop('error', 'Error', result.desciption);
                }
            }, error => {
                this.itemmodel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
        }

        this.isLoading = true;
    }

    getItems() {
        if (this.lcdaModel.id.length < 1) {
            return;
        }
        this.isLoading = true;
        this.itemService.getByLcdaId(this.lcdaModel.id, this.pageModel)
            .subscribe(response => {
                const objSchema = { data: [], totalPageCount: 1 };
                const result = Object.assign(objSchema, response);
                this.items = result.data;
                this.pageModel.totalPageCount = result.totalPageCount;
                this.isLoading = false;
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.items.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getItems();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getItems();
    }
}