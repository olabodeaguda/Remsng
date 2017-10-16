import { Injectable } from '@angular/core';
import { DataService } from "../../shared/services/data.service";
import { SectorModel } from "../models/sector.model";

@Injectable()
export class SectorService {

    constructor(private dataservice: DataService) {
    }

    getSectorByLcdaId(id: string) {
        return this.dataservice.get('sector/bylcda/' + id).catch(x => this.dataservice.handleError(x));
    }

    getSectorById(id: string) {
        return this.dataservice.get('sector/' + id).catch(x => this.dataservice.handleError(x));
    }
    add(sector: SectorModel) {
        return this.dataservice.post('sector/', {
            lcdaId: sector.lcdaId,
            sectorName: sector.sectorName
        }).catch(x => this.dataservice.handleError(x));
    }

    update(sector: SectorModel) {
        return this.dataservice.put('sector/' + sector.id, {
            lcdaId: sector.lcdaId,
            sectorName: sector.sectorName,
            id: sector.id
        }).catch(x => this.dataservice.handleError(x));
    }
}
