import {Component} from '@angular/core';
import { ICarouselConfig, AnimationConfig } from 'angular4-carousel';

@Component({
    selector:'',
    templateUrl:'../views/dashboard-index.component.html',
    styleUrls: ['../css/dashboard-index.css'],
})

export class DashboardIndexComponent{

    public imageSources: string[] = [
        'assets/dist/img/landing.jpeg',
        'assets/dist/img/landing.jpeg'
     ];
     
     public config: ICarouselConfig = {
       verifyBeforeLoad: true,
       log: false,
       animation: true,
       animationType: AnimationConfig.SLIDE,
       autoplay: true,
       autoplayDelay: 1500,
       stopAutoplayMinWidth: 768
     };
}