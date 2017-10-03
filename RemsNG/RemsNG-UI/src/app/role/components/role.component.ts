import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { RoleService } from '../services/role.service';
import { RoleModel } from '../models/role.model';
import { ToasterService } from 'angular2-toaster';
import { AppSettings } from '../../shared/models/app.settings';
import { DomainService } from '../../domain/services/domain.service';
import { PageModel } from '../../shared/models/page.model';
import { ResponseModel } from '../../shared/models/response.model';
declare var jQuery: any;

@Component({
    selector: 'app-role',
    templateUrl: '../views/role.component.html'
})

export class RoleComponent implements OnInit {
    pageModel: PageModel;
    domainLst = [];
    roleLst = [];
    isLoading: boolean = false;
    roleModel: RoleModel;
    @ViewChild('addModal') addModal: ElementRef;
    @ViewChild('changestatus') changestatusModal: ElementRef;
    constructor(private roleService: RoleService, private appSettings: AppSettings,
        private toasterService: ToasterService, private domainservice: DomainService) {
        this.roleModel = new RoleModel();
        this.pageModel = new PageModel();
    }

    ngOnInit() {
        this.getRoles();
        this.getDomains();
    }

    getRoles() {

        this.isLoading = true;
        this.roleService.getRoles(this.pageModel).subscribe(response => {
            this.isLoading = false;
            const resultSchema = { data: [], totalPageCount: 0 };
            const result = Object.assign(resultSchema, response.json());
            this.pageModel.totalPageCount = result.totalPageCount;
            this.roleLst = result.data;
        },
            error => {
                this.isLoading = false;
            });
    }

    getDomains() {
        this.roleModel.isLoading = true;
        this.domainservice.CurrentDomain().subscribe(response => {
            this.roleModel.isLoading = false;
            const resultSchema = { isMosAdmin: false, domains: [] }
            const res = Object.assign(new ResponseModel(), response.json());
            const result = Object.assign(resultSchema, res.data);
            if (result.isMosAdmin) {
                this.domainLst = result.domains;
            }
        },
            error => {
                this.roleModel.isLoading = false;
            }
        )
    }

    roleAction() {
        if (this.roleModel.eventType === this.appSettings.addMode) {
            this.roleModel.isLoading = true;
            this.roleService.add(this.roleModel).subscribe(response => {
                this.roleModel.isLoading = false;
                const result = Object.assign(new ResponseModel(), response.json());
                if(result.code === "00") {
                    jQuery(this.addModal.nativeElement).modal('hide');
                    this.toasterService.pop('success', 'Success',result.description);
                    this.getRoles();
                } else {
                    this.toasterService.pop('error', 'error', result.description);
                }
            },
                error => {
                    this.roleModel.isLoading = false;
                    jQuery(this.addModal.nativeElement).modal('hide');
                });
        } else if(this.roleModel.eventType === this.appSettings.editMode) {
            this.roleModel.isLoading = true;
            this.roleService.update(this.roleModel).subscribe(response=>{
               this.notifyUI(response)
               jQuery(this.addModal.nativeElement).modal('hide');
            }, error => {
                this.roleModel.isLoading = false;
                jQuery(this.addModal.nativeElement).modal('hide');
            })
        } else if(this.roleModel.eventType === this.appSettings.changeStatusMode) {
            this.roleModel.isLoading = true;
            this.roleModel.roleStatus = this.roleModel.roleStatus === 'ACTIVE' ? 'NOT_ACTIVE' : 'ACTIVE';
            this.roleService.changeStatus(this.roleModel).subscribe(response =>{
                this.notifyUI(response);
                this.roleModel.isLoading = false;
            }, error =>{
                this.roleModel.isLoading = false
                jQuery(this.addModal.nativeElement).modal('hide');
            });
        }
    }

    open(eventType: string, data: any) {
        if (eventType === this.appSettings.editMode) {
            this.roleModel = data;
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.addMode) {
            this.roleModel = new RoleModel();
            jQuery(this.addModal.nativeElement).modal('show');
        } else if (eventType === this.appSettings.changeStatusMode) {
            this.roleModel = data;
             jQuery(this.changestatusModal.nativeElement).modal('show');
        }
        this.roleModel.eventType = eventType;
    }

    next() {
        if (this.pageModel.pageNum > 1 && this.roleLst.length < 1) {
            return;
        }
        this.pageModel.pageNum += 1;
        this.getRoles();
    }

    previous() {
        this.pageModel.pageNum -= 1;
        if (this.pageModel.pageNum < 1) {
            this.pageModel.pageNum = 1;
        }
        this.getRoles();
    }

    notifyUI(response: Response){
        const result = Object.assign(new ResponseModel(), response.json());
        if(result.code === "00") {
            this.toasterService.pop('success', 'Success',result.description);
            this.getRoles();
        } else {
            this.toasterService.pop('error', 'error', result.description);
        }
    }

}