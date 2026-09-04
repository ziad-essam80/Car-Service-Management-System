import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DashboardService } from './dashboard-service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {

  customer: any = null;

  totalCars = 0;
  activeBookings = 0;
  completedBookings = 0;

  upcomingBooking: any = null;

  loading = true;


  constructor(
    private dashboardService: DashboardService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}


  ngOnInit(): void {
    this.loadDashboard();
  }


  loadDashboard(): void {

    this.loading = true;

    this.dashboardService
      .getDashboard()
      .subscribe({

        next: (data) => {

          console.log('Dashboard Data:', data);

          this.customer = data.customer;

          this.totalCars = data.totalCars;

          this.activeBookings =
            data.activeBookings;

          this.completedBookings =
            data.completedBookings;

          this.upcomingBooking =
            data.upcomingBooking;

          this.loading = false;

          this.cdr.detectChanges();
        },

        error: (error) => {

          console.log(
            'Dashboard Error:',
            error
          );

          this.loading = false;

          this.cdr.detectChanges();
        }

      });

  }


  goToCars(): void {

    this.router.navigate([
      '/customer/cars'
    ]);

  }


  goToServices(): void {

    this.router.navigate([
      '/customer/services'
    ]);

  }


  goToBookings(): void {

    this.router.navigate([
      '/customer/bookings'
    ]);

  }


  trackBooking(): void {

    if (!this.upcomingBooking) {
      return;
    }

    this.router.navigate([
      '/customer/tracking',
      this.upcomingBooking.bookingId
    ]);

  }

}