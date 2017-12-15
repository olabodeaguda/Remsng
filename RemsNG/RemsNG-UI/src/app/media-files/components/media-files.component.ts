import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { ToasterService } from 'angular2-toaster';
import { ActivatedRoute } from '@angular/router';
import { LcdaService } from '../../lcda/services/lcda.services';
import { LcdaModel } from '../../lcda/models/lcda.models';
import { ResponseModel } from '../../shared/models/response.model';
import { PageModel } from '../../shared/models/page.model';
import { MediaFileModel } from '../models/media-file.model';
import { MediaFileService } from '../services/media-file.service';
import { error } from 'util';
import {DomSanitizer} from '@angular/platform-browser';
import { AppSettings } from '../../shared/models/app.settings';
declare var jQuery: any;

@Component({
    selector: 'app-media',
    templateUrl: '../views/media-file.component.html'
})

export class MediaFileComponent implements OnInit {
    imgLst = [];
    lcdaId = '';
    mediaModel: MediaFileModel = new MediaFileModel();
    lgda: LcdaModel = new LcdaModel();
    pageModel: PageModel = new PageModel();
    isLoading: boolean = false;
    isfileLoading: boolean = false;
    public imgTypes: string[] = ['COUNCIL_TREASURER_SIGNATURE', 'REVENUE_COORDINATOR_SIGNATURE', 'LOGO'];
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('removeModal') removeModal: ElementRef;

    constructor(
        private toasterService: ToasterService,
        private activeRoute: ActivatedRoute,
        private lcdaService: LcdaService,
        private mediaService: MediaFileService,
        private sanitizer:DomSanitizer,
        private appsettings:AppSettings) {
    }

    ngOnInit() {
        this.initialize();
    }

    initialize() {
        this.activeRoute.params.subscribe((param: any) => {
            this.lcdaId = param["lcdaId"];
        });
        this.getLcda();
        this.getImages();
    }

    sanitize(url:string){
        return this.sanitizer.bypassSecurityTrustUrl( this.appsettings.root_url+"/images/"+url);
    }

    getLcda() {
        if (this.lcdaId.length < 1) {
            return;
        }
        this.isLoading = true;
        this.lcdaService.getLCdaById(this.lcdaId).subscribe(response => {
            this.isLoading = false;
            const result = Object.assign(new ResponseModel(), response);
            if (result.code == '00') {
                this.lgda = Object.assign(new LcdaModel(), result.data);
                this.mediaModel.ownerId = this.lgda.id;
            }
        }, error => {
            this.isLoading = false;
        })
    }

    getImages() {
        if (this.lcdaId.length < 1) {
            return;
        }
        this.mediaService.getImagesByLcda(this.lcdaId).subscribe(response=>{
            this.imgLst = response;
        },error=>{
        })

    }

    openAdd() {
        jQuery(this.addModal.nativeElement).modal('show');
    }

    addImage() {
        if (this.lcdaId.length < 1) {
            return;
        } else if (this.lcdaId.length < 1) {
            return;
        } else if (this.mediaModel.imgBase64.length < 1) {
            this.toasterService.pop('Image Error', 'Image Error', "Please re-upload image!!!");
            return;
        }
this.mediaModel.isLoading = true;
        this.mediaService.addImage(this.mediaModel).subscribe(response=>{
            this.mediaModel.isLoading = false;
            if(response.code === '00'){
                this.getImages();
                this.toasterService.pop('success','Successfull',response.description);
                jQuery(this.addModal.nativeElement).modal('hide');
            }
            else{
                this.toasterService.pop('error','Error',response.description);
                jQuery(this.addModal.nativeElement).modal('hide');
            }
        },error=>{
            this.mediaModel.isLoading = false;
            this.toasterService.pop('error','Error',error);
            //jQuery(this.addModal.nativeElement).modal('hide');
        });
    }


    changeListener($event): void {
        if (this.lcdaId.length < 1) {
            this.toasterService.pop("error", "Error", "An error occur, please refresh you page and try again")
            jQuery('input[type=file]').val('');
            return;
        } else if (this.mediaModel.imgType.length < 1 || this.mediaModel.imgType === 'none') {
            this.toasterService.pop("error", "Error", "Type is required.")
            jQuery('input[type=file]').val('');
            return;
        }


        let inputValue = $event.target;
        if (inputValue.files.length < 1) {
            return;
        }

        let file: File = inputValue.files[0];
        let reader: FileReader = new FileReader();
        reader.onloadstart = (e) => {
            this.mediaModel.isLoading = true;
        };
        reader.onload = (e) => {
            if (file.size > (200 * 1024)) {
                this.toasterService.pop('error', 'Error', 'You can\'t attach more than 120KB file');
                jQuery('input[type=file]').val('');
                return;
            }

            this.mediaModel.imgFilename = file.name;
            this.mediaModel.imgBase64 = reader.result;

        };
        reader.onloadend = (e) => {
            this.mediaModel.isLoading = false;
        };
        reader.readAsDataURL(file);
    }


}