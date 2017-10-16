import {Component, OnInit} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LcdaService } from "../../lcda/services/lcda.services";
import { ToasterService } from "angular2-toaster/angular2-toaster";
import { LcdaModel } from "../../lcda/models/lcda.models";
import { ResponseModel } from "../../shared/models/response.model";
import { SectorService } from "../services/sector.services";

@Component({
    selector: 'app-sector',
    templateUrl: '../views/sector.component.html'
})

export class SectorComponent implements OnInit{
    lcdaModel: LcdaModel
    isLoading = false;
    sectors =[];
    constructor(private activeRoute: ActivatedRoute,
        private lcdaService:LcdaService,  
        private toasterService: ToasterService,
    private sectorService: SectorService) {        
            this.lcdaModel = new LcdaModel();
    }

    ngOnInit(): void {
        this.initializePage();
    }

    initializePage() {
        this.activeRoute.params.subscribe((param: any) => {
            this.getLcda(param["id"]);
        });
    }
    getLcda(lcdaId: string){
        this.isLoading = true;
        this.lcdaService.getLCdaById(lcdaId).subscribe(response=>{
            this.isLoading = false;
            const objSchema  = Object.assign(new ResponseModel(),response.json());
            if(objSchema.code == '00'){
                this.lcdaModel = objSchema.data;
                this.getSectors();
            }
            else{
                this.toasterService.pop('error','Error',objSchema.desciption);
            }

        }, error=>{
            this.toasterService.pop('error','Error',error);
        })
    }

    getSectors(){
        this.isLoading = true;
        this.sectorService.getSectorByLcdaId(this.lcdaModel.id)
        .subscribe(response=>{
            this.isLoading = false;
            this.sectors = response.json();
        }, error=>{
            this.isLoading = false;
            this.toasterService.pop('error','Error',error);
        })
    }
}