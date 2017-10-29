import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LcdaService } from "../../lcda/services/lcda.services";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { LcdaModel } from "../../lcda/models/lcda.models";
import { ResponseModel } from "../../shared/models/response.model";
import { SectorService } from "../services/sector.services";
import { SectorModel } from "../models/sector.model";
declare var jQuery: any;

@Component({
    selector: 'app-sector',
    templateUrl: '../views/sector.component.html'
})

export class SectorComponent implements OnInit {
    lcdaModel: LcdaModel
    isLoading = false;
    sectors = [];
    sectorModel: SectorModel;
    @ViewChild('addModal') addModal: ElementRef;
    constructor(private activeRoute: ActivatedRoute,
        private lcdaService: LcdaService,
        private toasterService: ToasterService,
        private sectorService: SectorService) {
        this.lcdaModel = new LcdaModel();
        this.sectorModel = new SectorModel();
    }

    ngOnInit(): void {
        this.initializePage();
    }

    initializePage() {
        this.activeRoute.params.subscribe((param: any) => {
            this.getLcda(param["id"]);
        });
    }
    
    getLcda(lcdaId: string) {
        this.isLoading = true;
        this.lcdaService.getLCdaById(lcdaId).subscribe(response => {
            this.isLoading = false;
            const objSchema = Object.assign(new ResponseModel(), response);
            if (objSchema.code == '00') {
                this.lcdaModel = objSchema.data;
                this.getSectors();
            }
            else {
                this.toasterService.pop('error', 'Error', objSchema.desciption);
            }

        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    getSectors() {
        this.isLoading = true;
        this.sectorService.getSectorByLcdaId(this.lcdaModel.id)
            .subscribe(response => {
                this.isLoading = false;
                this.sectors = response;
            }, error => {
                this.isLoading = false;
                this.toasterService.pop('error', 'Error', error);
            })
    }

    open(eventType: string, data) {
        if (eventType === 'ADD') {
            this.sectorModel = new SectorModel();
            jQuery(this.addModal.nativeElement).modal('show');
        } else if(eventType === 'EDIT'){
            this.sectorModel = data;
            jQuery(this.addModal.nativeElement).modal('show');
        }

        this.sectorModel.eventType = eventType;
    }

    actions() {
        if (this.sectorModel.sectorName.length < 1) {
            this.toasterService.pop('error', 'Error', 'Sector Name is required');
            return;
        }
        this.sectorModel.lcdaId = this.lcdaModel.id;

        if (this.sectorModel.eventType === 'ADD') {
            this.sectorModel.isLoading = true;
            this.sectorService.add(this.sectorModel).subscribe(response => {
                this.sectorModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    this.getSectors();
                    jQuery(this.addModal.nativeElement).modal('hide');                    
                }
            }, error => {
                this.sectorModel.isLoading = false;
                jQuery(this.addModal.nativeElement).modal('hide');
                this.toasterService.pop('error', 'Error', error);
            })
        } else if (this.sectorModel.eventType === 'EDIT') {
            this.sectorModel.isLoading = true;          
            this.sectorService.update(this.sectorModel).subscribe(response=>{
                const result = Object.assign(new ResponseModel(), response);
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    this.getSectors();
                    jQuery(this.addModal.nativeElement).modal('hide');                    
                }
            },error=>{
                this.getSectors();
                jQuery(this.addModal.nativeElement).modal('hide'); 
                this.sectorModel.isLoading = false;
                this.toasterService.pop('error','Error')
            });
        }
    }
}