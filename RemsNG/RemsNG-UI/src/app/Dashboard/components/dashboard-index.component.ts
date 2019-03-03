import { Component, OnInit } from '@angular/core';
import { ICarouselConfig, AnimationConfig } from 'angular4-carousel';
import { Router } from '@angular/router';
import { StorageService } from '../../shared/services/storage.service';

@Component({
    selector: '',
    templateUrl: '../views/dashboard-index.component.html',
    styleUrls: ['../css/dashboard-index.css'],
})

export class DashboardIndexComponent implements OnInit {

    public imageSources: string[] = [
        'assets/dist/img/landing2.jpg',
        'assets/dist/img/landing2.jpg'
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

    constructor(private router: Router,
        private storageService: StorageService) {
    }

    ngOnInit() {
        if (this.storageService.get() !== null)
            this.router.navigate(['/dashboard']);
    }
}