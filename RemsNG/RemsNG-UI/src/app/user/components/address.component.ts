import { Component, Input, OnChanges, SimpleChanges, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ProfileModel } from "../models/profile.model";
import { StreetService } from "../../street/services/street.service";
import { AddressService } from "../services/address.service";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { AddressModel } from "../models/address.model";
import { ResponseModel } from "../../shared/models/response.model";
declare var jQuery: any;

@Component({
    selector: 'address-comp',
    templateUrl: '../views/address.component.html'
})

export class AddressComponent implements OnChanges, OnInit {

    @Input()
    profileModel: ProfileModel;
    addresses = [];
    streets = [];
    addressModel: AddressModel;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('removeAddressModal') removeAddressModal: ElementRef;
    constructor(private streetservice: StreetService,
        private addressService: AddressService, private toasterService: ToasterService) {
        this.addressModel = new AddressModel();
    }

    ngOnChanges(changes: SimpleChanges): void {
        //   this.profileModel = changes.profileModel.currentValue;
    }

    ngOnInit(): void {
        this.getAddresses();
        this.getStreet();
    }

    getAddresses() {
        this.addressService.byOwnerId(this.profileModel.id, this.profileModel.lcdaId)
            .subscribe(response => {
                this.addresses = Object.assign([], response.json());
            }, error => {
                this.toasterService.pop('error', 'Error', error);
            })
    }

    getStreet() {
        if (this.profileModel.lcdaId.length < 1) {
            return;
        }
        this.streetservice.bylcda(this.profileModel.lcdaId).subscribe(
            response => {
                this.streets = Object.assign([], response.json());
            }, error => {

            }
        );
    }

    open(eventType: string, data: any) {
        if (eventType == 'ADD') {
            this.addressModel = new AddressModel();
            this.addressModel.ownerId = this.profileModel.id;
            this.addressModel.lcdaid = this.profileModel.lcdaId;
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
                const result = Object.assign(new ResponseModel(), response.json());
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
                const result = Object.assign(new ResponseModel(), response.json());
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
                const result = Object.assign(new ResponseModel(), response.json());
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