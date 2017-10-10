import {Component, Output,Input} from '@angular/core';
import { ContactModel } from "../models/contact.model";
import { AppSettings } from "../../shared/models/app.settings";

@Component({
    selector: 'add-contact',
    templateUrl: '../views/add-contact.component.html'
})

export class AddContactComponent{
    @Input() contactModel: ContactModel;
    currentReg: string;
    constructor(private appSettings: AppSettings) {
        this.contactModel = new ContactModel();           
    }

    onChange(selectedValue){
        if(selectedValue === 'PHONENUMBER'){
            this.currentReg = this.appSettings.emailPattern
        } else if(selectedValue === 'EMAIL'){
            
            this.currentReg = this.appSettings.emailPattern
        }
    }
}