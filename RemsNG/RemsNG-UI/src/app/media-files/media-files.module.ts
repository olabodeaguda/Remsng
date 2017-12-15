import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule} from '@angular/forms';
import { BrowserModule} from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { LaddaModule } from 'angular2-ladda';
import { SharedModule } from '../shared/shared.module';
import { UserModule } from "../user/user.module";
import { MediaFileComponent } from './components/media-files.component';
import { MediaFileService } from './services/media-file.service';

const appRoutes: Routes = [
    { path: 'media/:lcdaId', component: MediaFileComponent }
 ];

@NgModule({
    imports: [
      BrowserModule,
      LaddaModule,
      FormsModule,
      ReactiveFormsModule,
      SharedModule,
      RouterModule.forChild(appRoutes)
    ],
    declarations: [
        MediaFileComponent
    ],
    providers: [ MediaFileService ],
    exports: [
        MediaFileComponent
    ]
  })

export class MediaFilesModule {

}
