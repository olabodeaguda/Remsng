import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { CategoryService } from "../services/category.service";
import { ActivatedRoute } from '@angular/router';
import { LcdaModel } from "../../lcda/models/lcda.models";
import { LcdaService } from "../../lcda/services/lcda.services";
import { ResponseModel } from "../../shared/models/response.model";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { CategoryModel } from "../models/category.model";
declare var jQuery: any;

@Component({
    selector: 'app-category',
    templateUrl: '../views/category.component.html'
})

export class CategoryComponent implements OnInit {
    lcdaModel: LcdaModel;
    categories = [];
    isLoading: boolean = false;
    categoryModel: CategoryModel
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('removeModal') removeModal: ElementRef;

    constructor(private categoryService: CategoryService,
        private activeRoute: ActivatedRoute, private lcdaService: LcdaService,
        private toasterService: ToasterService) {
        this.lcdaModel = new LcdaModel();
        this.categoryModel = new CategoryModel();
    }

    ngOnInit(): void {
        this.initialize();
    }

    getLcda(lcdaId: string) {
        this.isLoading = true;
        this.lcdaService.getLCdaById(lcdaId).subscribe(response => {
            this.isLoading = false;
            const objSchema = Object.assign(new ResponseModel(), response.json());
            if (objSchema.code == '00') {
                this.lcdaModel = objSchema.data;
                this.getCategories();
            }
            else {
                this.toasterService.pop('error', 'Error', objSchema.desciption);
            }

        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        })
    }

    initialize() {
        this.activeRoute.params.subscribe((param: any) => {
            this.getLcda(param["id"]);
            this.getCategories();
        });
    }

    getCategories() {
        if (this.lcdaModel.id.length < 1) {
            return;
        }

        this.isLoading = true;
        this.categoryService.getAll(this.lcdaModel.id).subscribe(response => {
            this.isLoading = false;
            this.categories = response.json();
        }, error => {
            this.isLoading = false;
            this.toasterService.pop('error', 'Error', error);
        });
    }

    open(eventType: string, data) {
        if (eventType === 'ADD') {
            this.categoryModel = new CategoryModel();
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === 'EDIT') {
            this.categoryModel = data;
            jQuery(this.addModal.nativeElement).modal('show');
        }

        this.categoryModel.eventType = eventType;
    }

    actions() {
        if (this.categoryModel.taxpayerCategoryName.length < 1) {
            this.toasterService.pop('error', 'Error', 'Sector Name is required');
            return;
        }
        this.categoryModel.lcdaId = this.lcdaModel.id;

        if (this.categoryModel.eventType === 'ADD') {
            this.categoryModel.isLoading = true;
            this.categoryService.add(this.categoryModel).subscribe(response => {
                this.categoryModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    this.getCategories();
                    jQuery(this.addModal.nativeElement).modal('hide');                    
                }
            }, error => {
                this.categoryModel.isLoading = false;
                jQuery(this.addModal.nativeElement).modal('hide');
                this.toasterService.pop('error', 'Error', error);
            })
        } else if (this.categoryModel.eventType === 'EDIT') {
            this.categoryModel.isLoading = true;          
            this.categoryService.update(this.categoryModel).subscribe(response=>{
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    this.getCategories();
                    jQuery(this.addModal.nativeElement).modal('hide');                    
                }
            },error=>{
                this.getCategories();
                jQuery(this.addModal.nativeElement).modal('hide'); 
                this.categoryModel.isLoading = false;
                this.toasterService.pop('error','Error')
            });
        } else if(this.categoryModel.eventType === 'REMOVE'){
            this.categoryModel.isLoading = true;
            this.categoryService.remove(this.categoryModel.id).subscribe(response=>{
                const result = Object.assign(new ResponseModel(), response.json());
                if (result.code === '00') {
                    this.toasterService.pop('success', 'Success', result.description);
                    this.getCategories();
                    jQuery(this.removeModal.nativeElement).modal('hide');                    
                }
            },error=>{
                this.getCategories();
                jQuery(this.removeModal.nativeElement).modal('hide'); 
                this.categoryModel.isLoading = false;
                this.toasterService.pop('error','Error')
            });
        }

    }

}