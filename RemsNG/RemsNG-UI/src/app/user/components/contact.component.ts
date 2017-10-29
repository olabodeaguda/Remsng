import { Component, Input, ElementRef, ViewChild, OnInit, OnChanges, SimpleChanges, SimpleChange } from '@angular/core';
import { ProfileModel } from "../models/profile.model";
import { ContactModel } from "../models/contact.model";
import { AppSettings } from "../../shared/models/app.settings";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { EmailValidator } from '@angular/forms';
import { UserService } from "../services/user.service";
import { ResponseModel } from "../../shared/models/response.model";
import { ContactService } from "../services/contact.service";
declare var jQuery: any;

@Component({
    selector: 'user-contact',
    templateUrl: '../views/contact.component.html'
})

export class ContactComponent implements OnChanges {
    
    private _profileModel: ProfileModel;
  
    @Input() set profileModel(value: ProfileModel){
        this._profileModel = value;
        this.getStreet(value.lcdaId);
    }

    get profileModel():ProfileModel{
        return this._profileModel;
    }

    contactModel: ContactModel;
    contactLst = [];
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('removeContact') removeContact: ElementRef;

    constructor(private appSettings: AppSettings,
        private toasterService: ToasterService, private userService: UserService,
        private contactService: ContactService) {
        this.contactModel = new ContactModel();
    }
   
    ngOnChanges(changes: SimpleChanges): void {
        this.getContact(this._profileModel);
    }

    open(eventType: string, data: any) {
        if (eventType === this.appSettings.addMode) {
            this.contactModel = new ContactModel();
            this.contactModel.ownerId = this.profileModel.id;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.editMode) {
            this.contactModel = data;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.removeMode) {
            this.contactModel = data;
            jQuery(this.removeContact.nativeElement).modal('show');
        }

        this.contactModel.eventType = eventType;
    }

    getContact(pf: ProfileModel) {
        if (pf.id.length < 1) {
            return
        }
        this.contactService.getContactsDetails(pf.id).subscribe(response => {
            this.contactLst = response;
        }, error => {
            this.toasterService.pop('error', 'Error', error);
        })

    }

    getStreet(lcdaId: string){
        // console.log('in street '+lcdaId);
    }

    contactAction() {
        if (this.contactModel.contactType === 'EMAIL') {
            if (!this.appSettings.validatEmail(this.contactModel.contactValue)) {
                this.toasterService.pop('error', 'Validation error', 'Email is invalid');
                return;
            }
        } else if (this.contactModel.contactType === 'PHONENUMBER') {
            if (!this.appSettings.validatePhoneNumber(this.contactModel.contactValue)) {
                this.toasterService.pop('error', 'Validation error', 'Phone number is invalid');
                return;
            }
        }
        if (this.profileModel.id.length < 1) {
            this.toasterService.pop('error', 'Validation error', 'Can not identifier the selected user. Please, restart the process');
            return;
        }

        this.contactModel.ownerId = this.profileModel.id;
        this.contactModel.isLoading = true;
        if (this.contactModel.eventType === this.appSettings.addMode) {
            this.contactService.addContact(this.contactModel).subscribe(response => {
                this.contactModel.isLoading = false;
                const result: ResponseModel = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', 'contact have been added');
                    this.getContact(this.profileModel);
                }
                jQuery(this.addModal.nativeElement).modal('hide');
            }, error => {
                this.contactModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            });
        } else if (this.contactModel.eventType === this.appSettings.editMode) {
            this.contactService.update(this.contactModel).subscribe(response => {
                this.contactModel.isLoading = false;
                const result: ResponseModel = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', 'Updated was successful');
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.getContact(this.profileModel);
                }
                jQuery(this.addModal.nativeElement).modal('hide');
            }, error => {
                this.contactModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })
        } else if (this.contactModel.eventType === this.appSettings.removeMode) {
            this.contactService.remove(this.contactModel.id).subscribe(response => {
                this.contactModel.isLoading = false;
                const result: ResponseModel = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    this.getContact(this.profileModel);
                }
                jQuery(this.removeContact.nativeElement).modal('hide');
            }, error => {
                this.contactModel.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })

        }
    }
}