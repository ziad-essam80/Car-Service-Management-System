import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import { ServicesService } from '../services/services-service';
import { CarsService } from '../cars/cars-service';
import { BookingsService } from '../bookings/bookings-service';

@Component({
  selector: 'app-book-service',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './book-service.html',
  styleUrl: './book-service.css'
})
export class BookService implements OnInit {

  bookingForm!: FormGroup;

  cars: any[] = [];

  services: any[] = [];

  selectedService: any = null;

  minDate = '';

  loading = false;

constructor(
  private fb: FormBuilder,
  private route: ActivatedRoute,
  private router: Router,
  private servicesService: ServicesService,
  private carsService: CarsService,
  private bookingsService: BookingsService,
  private cdr : ChangeDetectorRef
) {}

  ngOnInit(): void {

    const today = new Date();

    this.minDate =
      today.toISOString().split('T')[0];


    const serviceId =
      Number(
        this.route.snapshot.queryParamMap.get('serviceId')
      );


    this.bookingForm = this.fb.group({

      carId: [
        '',
        Validators.required
      ],

      serviceId: [
        serviceId || '',
        Validators.required
      ],

      date: [
        '',
        Validators.required
      ],

      time: [
        '',
        Validators.required
      ]

    });


    this.loadCars();

    this.loadServices();


    if (serviceId) {
      this.loadSelectedService(serviceId);
    }

  }


  loadCars(): void {

    this.carsService
      .getCars()
      .subscribe({

        next: (data) => {

          this.cars = data;
          this.cdr.detectChanges() 
        },

        error: (error) => {

          console.log(error);

        }

      });

  }


  loadServices(): void {

    this.servicesService
      .getServices()
      .subscribe({

        next: (data) => {

          this.services = data;

        },

        error: (error) => {

          console.log(error);

        }

      });

  }


  loadSelectedService(id: number): void {

    this.servicesService
      .getService(id)
      .subscribe({

        next: (data) => {

          this.selectedService = data;

        },

        error: (error) => {

          console.log(error);

        }

      });

  }


  serviceChanged(): void {

    const serviceId =
      Number(
        this.bookingForm.value.serviceId
      );


    if (!serviceId) {

      this.selectedService = null;

      return;

    }


    this.loadSelectedService(serviceId);

  }


  confirmBooking(): void {

    if (this.bookingForm.invalid) {

      this.bookingForm.markAllAsTouched();

      return;

    }


    this.loading = true;


    const booking = {

      carId: Number(
        this.bookingForm.value.carId
      ),

      serviceId: Number(
        this.bookingForm.value.serviceId
      ),

      date:
        this.bookingForm.value.date,

      time:
        this.bookingForm.value.time

    };


    this.bookingsService
      .createBooking(booking)
      .subscribe({

        next: () => {

          this.loading = false;

          alert(
            'Booking created successfully'
          );

          this.router.navigate([
            '/customer/bookings'
          ]);

        },

        error: (error) => {

          this.loading = false;

          alert(
            error.error?.message ||
            'Booking failed'
          );

        }

      });

  }


  goBack(): void {

    this.router.navigate([
      '/customer/services'
    ]);

  }

}