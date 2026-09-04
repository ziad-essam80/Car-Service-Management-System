import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { BookingsService } from './bookings-service';

@Component({
  selector: 'app-bookings',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './bookings.html',
  styleUrl: './bookings.css'
})
export class Bookings implements OnInit {

  bookings: any[] = [];
  filteredBookings: any[] = [];

  loading = true;

  selectedFilter = 'all';

  constructor(
    private bookingsService: BookingsService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {

    this.loading = true;

    this.bookingsService
      .getBookings()
      .subscribe({

        next: (data: any) => {

          console.log(
            'Bookings API Response:',
            data
          );

          if (Array.isArray(data)) {

            this.bookings = data;

          } else if (data?.$values) {

            this.bookings = data.$values;

          } else if (data?.data) {

            this.bookings = data.data;

          } else {

            this.bookings = [];

          }

          this.filteredBookings = [
            ...this.bookings
          ];

          this.loading = false;

          console.log(
            'Bookings:',
            this.bookings
          );

          console.log(
            'Loading:',
            this.loading
          );

          this.cdr.detectChanges();
        },

        error: (error) => {

          console.error(
            'Bookings API Error:',
            error
          );

          this.bookings = [];
          this.filteredBookings = [];

          this.loading = false;

          this.cdr.detectChanges();
        }

      });

  }

  filterBookings(filter: string): void {

    this.selectedFilter = filter;

    if (filter === 'all') {

      this.filteredBookings = [
        ...this.bookings
      ];

    } else if (filter === 'active') {

      this.filteredBookings =
        this.bookings.filter(
          booking =>
            booking.status !== 'Completed'
        );

    } else if (filter === 'completed') {

      this.filteredBookings =
        this.bookings.filter(
          booking =>
            booking.status === 'Completed'
        );

    }

    this.cdr.detectChanges();
  }

  trackBooking(id: number): void {

    this.router.navigate([
      '/customer/tracking',
      id
    ]);

  }

  viewInvoice(bookingId: number): void {

    this.router.navigate([
      '/customer/invoice',
      bookingId
    ]);

  }

  getStatusClass(status: string): string {

    switch (status) {

      case 'Booked':
        return 'booked';

      case 'Confirmed':
        return 'confirmed';

      case 'Vehicle Received':
        return 'received';

      case 'Inspection':
        return 'inspection';

      case 'In Service':
        return 'service';

      case 'Quality Check':
        return 'quality';

      case 'Ready for Pickup':
        return 'ready';

      case 'Completed':
        return 'completed';

      default:
        return 'default';

    }

  }

}