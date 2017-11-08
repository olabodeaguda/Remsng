import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { WardModel } from "../../ward/models/ward.model";
import { StreetService } from "../services/street.service";
import { WardService } from "../../ward/services/ward.service";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { StreetModel } from "../models/street.model";
import { PageModel } from "../../shared/models/page.model";
import { ActivatedRoute } from '@angular/router';
import { ResponseModel } from "../../shared/models/response.model";
declare var jQuery: any;

@Component({
    selector: 'app-street',
    templateUrl: '../views/street.component.html'
})

export class StreetComponent implements OnInit {
    streetModel: StreetModel;
    wardmodel: WardModel;
    isLoading: boolean = false;
    streetModels = [];
    pageModel: PageModel;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changestatus') changestatusModal: ElementRef;

    constructor(private activeRoute: ActivatedRoute,
        private streetservice: StreetService,
        private wardService: WardService, private toasterService: ToasterService) {
        this.wardmodel = new WardModel();
        this.pageModel = new PageModel();
        this.streetModel = new StreetModel();
    }

    ngOnInit(): void {
        this.activeRoute.params.subscribe((param: any) => {
            this.getWard(param["id"]);
        });
    }

    getWard(id: string) {
        this.isLoading = true;
        this.wardService.byId(id).subscribe(response => {
            this.isLoading = false;
            this.wardmodel = Object.assign(new WardModel(), response);
            this.getStreet();
        }, error => {
            this.toasterService.pop('error', 'Error', error);
        })
    }

    getStreet() {
        if (this.wardmodel.id.length < 1) {
            return;
        }

        this.isLoading = true;
        this.streetservice.byWardIdpaginated(this.wardmodel.id, this.pageModel).subscribe(response => {
            this.isLoading = false;
            const objSchema = { data: [], totalPageCount: 0 };
            const result = Object.assign(objSchema, response);
            if (result.data.length > 0) {
                this.streetModels = Object.assign([], result.data)
                this.pageModel.totalPageCount = (objSchema.totalPageCount % this.pageModel.pageSize > 0 ? 1 : 0) + Math.floor(objSchema.totalPageCount / this.pageModel.pageSize);
            } else {
                this.pageModel.pageNum -= 1;
            }
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.streetModels.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getStreet();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getStreet();
    }

    open(eventType: string, data: any) {
        if (eventType === 'ADD') {
            this.streetModel = new StreetModel();
            this.streetModel.wardId = this.wardmodel.id;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === 'EDIT') {
            this.streetModel = data
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === 'CHANGE_STATUS') {
            this.streetModel = data
            jQuery(this.changestatusModal.nativeElement).modal('show');
        }
        this.streetModel.eventType = eventType;
    }

    actions() {
        if (this.streetModel.streetName.length < 1) {
            this.toasterService.pop('error', 'Error', 'Street Name is required');
            return;
        }
        this.streetModel.isLoading = true;
        if (this.streetModel.eventType === 'ADD') {
            this.streetservice.add(this.streetModel).subscribe(response => {
                this.streetModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code == '00') {
                    this.getStreet();
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                }
            }, error => {
                this.streetModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
        } else if (this.streetModel.eventType === 'EDIT') {
            this.streetservice.update(this.streetModel).subscribe(response => {
                this.streetModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code == '00') {
                    this.getStreet();
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.addModal.nativeElement).modal('hide');
                }
            }, error => {
                this.streetModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                jQuery(this.addModal.nativeElement).modal('hide');
            })
        } else if (this.streetModel.eventType === 'CHANGE_STATUS') {
            this.streetModel.streetStatus = this.streetModel.streetStatus === 'ACTIVE' ? 'NOT_ACTIVE' : 'ACTIVE';
            this.streetservice.changeStatus(this.streetModel).subscribe(response => {
                this.streetModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code == '00') {
                    this.getStreet();
                    this.toasterService.pop('success', 'Success', result.description);
                    jQuery(this.changestatusModal.nativeElement).modal('hide');
                }
            }, error => {
                this.streetModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
                jQuery(this.changestatusModal.nativeElement).modal('hide');
            })
        }
    }

}
