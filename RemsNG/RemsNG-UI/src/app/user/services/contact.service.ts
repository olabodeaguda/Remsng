import { Injectable } from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { ContactModel } from "../models/contact.model";


@Injectable()
export class ContactService {

    constructor(private dataService: DataService) {
    }

    addContact(contactModel: ContactModel) {
        return this.dataService.post('contact', {
            ownerId: contactModel.ownerId,
            contactValue: contactModel.contactValue,
            contactType: contactModel.contactType
        }).catch(error => this.dataService.handleError(error));
    }

    getContactsDetails(id: string) {
        return this.dataService.get('contact/' + id
        ).catch(error => this.dataService.handleError(error));
    }

    update(contactModel: ContactModel) {
        return this.dataService.put('contact/' + contactModel.id, {
            ownerId: contactModel.ownerId,
            contactValue: contactModel.contactValue,
            contactType: contactModel.contactType,
            id: contactModel.id
        }).catch(error => this.dataService.handleError(error));
    }

    remove(id: string) {
        return this.dataService.delete('contact/' + id).catch(error => this.dataService.handleError(error));
    }
}