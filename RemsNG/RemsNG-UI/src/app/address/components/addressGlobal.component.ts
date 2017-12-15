import { Component, Input, OnChanges, SimpleChanges, OnInit, ElementRef, ViewChild } from '@angular/core';
import { StreetService } from "../../street/services/street.service";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { ResponseModel } from "../../shared/models/response.model";
import { AddressGlobalModel } from '../models/addressGlobal.model';
import { AddressGlobalService } from '../services/addressGlobal.service';
import { ActivatedRoute } from '@angular/router';
import { LcdaService } from '../../lcda/services/lcda.services';
import { LcdaModel } from '../../lcda/models/lcda.models';
declare var jQuery: any;

@Component({
    selector: 'address-comp',
    templateUrl: '../views/addressGlobal.component.html'
})

export class AddressGlobalComponent implements OnInit {
    addresses = [];
    streets = [];
    lcdaId='';
    ownerId = '';
    lgda:LcdaModel = new LcdaModel();
    isAddAddress:boolean = true;
    addressModel: AddressGlobalModel = new AddressGlobalModel();
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('removeAddressModal') removeAddressModal: ElementRef;
    isLoading:boolean = false;
    constructor(private streetservice: StreetService,
        private addressService: AddressGlobalService, 
        private toasterService: ToasterService,
        private activeRoute: ActivatedRoute,
    private lcdaService:LcdaService) {
    }

    ngOnInit(): void {
       this.initialize();
        }

    initialize(){
        this.activeRoute.params.subscribe((param: any) => {
            this.lcdaId = param["lcdaId"];
            this.ownerId = param["ownerId"];
        });
        this.getAddresses();
        this.getStreet();
        this.getLcda();
    }

    getAddresses(){
        this.isLoading = true;
        this.addressService.byOwnerId(this.ownerId, this.lcdaId)
            .subscribe(response => {
                this.isLoading=false;
                this.addresses = Object.assign([], response);
                if(this.addresses.length > 0){
                    this.isAddAddress = false;
                }
            }, error => {
                this.isLoading=false;
                this.toasterService.pop('error', 'Error', error);
            });
    }

    getLcda() {
        if(this.lcdaId.length < 1) {
            return;
        }
        this.lcdaService.getLCdaById(this.lcdaId).subscribe(response => {

            const result = Object.assign(new ResponseModel(), response);
            if (result.code == '00') {
                this.lgda = Object.assign(new LcdaModel(), result.data);
            }
        }, error => {

        })
    }

    getStreet() {
        if(this.lcdaId.length < 1) {
            return;
        }
        this.isLoading=true;
        this.streetservice.bylcda(this.lcdaId).subscribe(
            response => {
                this.isLoading=false;
                this.streets = Object.assign([], response);
               
            }, error => {
                this.isLoading=false;
                this.toasterService.pop('error', 'Error', error);
            }
        );
    }

    open(eventType: string, data: any) {
        if (eventType === 'ADD') {
            this.addressModel = new AddressGlobalModel();
            this.addressModel.ownerId = this.ownerId;
            this.addressModel.lcdaid = this.lcdaId;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if(eventType == 'EDIT'){
            this.addressModel = data;            
            jQuery(this.addModal.nativeElement).modal('show');
        }else if(eventType == 'REMOVE'){
            this.addressModel = data;            
            jQuery(this.removeAddressModal.nativeElement).modal('show');
        }
        this.addressModel.eventType = eventType;
    }

    actions() {
        if (this.addressModel.lcdaid.length < 1) {
            this.toasterService.pop('error', 'Error', 'Please refresh and try again');
            return;
        } else if (this.addressModel.ownerId.length < 1) {
            this.toasterService.pop('error', 'Error', 'Please refresh and try again');
            return;
        } else if (this.addressModel.streetId.length < 1) {
            this.toasterService.pop('error', 'Error', 'Street is required');
            return;
        } else if (this.addressModel.addressnumber.trim().length < 1) {

            this.toasterService.pop('error', 'Error', 'Address number is required');
            return;
        }
        this.addressModel.isLoading = true;
        if (this.addressModel.eventType == 'ADD') {
            this.addressService.add(this.addressModel).subscribe(response => {
                this.addressModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.getAddresses();
                    jQuery(this.addModal.nativeElement).modal('hide');
                } else {
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.addressModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })
        } else if(this.addressModel.eventType === 'EDIT'){
            this.addressService.update(this.addressModel).subscribe(response => {
                this.addressModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.getAddresses();
                    jQuery(this.addModal.nativeElement).modal('hide');
                } else {
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.addressModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })
        } else if(this.addressModel.eventType === 'REMOVE'){
            this.addressService.remove(this.addressModel.id).subscribe(response => {
                this.addressModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.getAddresses();
                    jQuery(this.removeAddressModal.nativeElement).modal('hide');
                } else {
                    this.toasterService.pop('error', 'Error', result.description);
                }
            }, error => {
                this.addressModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })
        }
    }

}