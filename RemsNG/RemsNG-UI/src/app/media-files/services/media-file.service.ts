import { Injectable } from '@angular/core';
import { MediaFileModel } from '../models/media-file.model';
import { DataService } from '../../shared/services/data.service';

@Injectable()
export class MediaFileService {

    constructor(private dataService: DataService) {
    }

    getImagesByLcda(lcdaId: string) {
        return this.dataService.get('media/ownerid/'+lcdaId)
        .catch(error=> this.dataService.handleError(error))
    }

    addImage(mediaImage: MediaFileModel) {
        return this.dataService.post('media', {
            imgFilename: mediaImage.imgFilename,
            ownerId: mediaImage.ownerId,
            imgType: mediaImage.imgType,
            imgBase64: mediaImage.imgBase64
        }).catch(error => this.dataService.handleError(error));
    }

}