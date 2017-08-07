import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AlertModule } from 'ngx-bootstrap';
import { AppComponent } from './app.component';
import {LoginModule} from './login/login.module';
import {DashBoardModule} from './Dashboard/dashboard.module';

@NgModule({
  declarations: [
      AppComponent
  ],
  imports: [
      BrowserModule,
      LoginModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
