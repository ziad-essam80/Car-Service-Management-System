import { Component, OnInit, ChangeDetectorRef } from '@angular/core';

import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ServicesService } from './services-service';

@Component({
  selector: 'app-services',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './services.html',
  styleUrl: './services.css',
})
export class Services implements OnInit {
  services: any[] = [];

  loading = true;

  constructor(
    private servicesService: ServicesService,
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {}
  

  ngOnInit(): void {
    this.loadServices();
  }

  loadServices(): void {
    this.loading = true;

    this.servicesService.getServices().subscribe({
      next: (data) => {
        console.log('Services Data:', data);

        this.services = data;

        this.loading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.log('Services Error:', error);

        this.services = [];

        this.loading = false;

        this.cdr.detectChanges();
      },
    });
  }

  viewDetails(id: number): void {
    this.router.navigate(['/customer/services', id]);
  }

  bookService(id: number): void {
    this.router.navigate(['/customer/book-service'], {
      queryParams: {
        serviceId: id,
      },
    });
  }
}
