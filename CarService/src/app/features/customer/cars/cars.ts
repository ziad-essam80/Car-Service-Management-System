import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { CarsService } from './cars-service';

@Component({
  selector: 'app-cars',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './cars.html',
  styleUrl: './cars.css',
})
export class Cars implements OnInit {
  cars: any[] = [];
  carForm!: FormGroup;

  loading = true;

  showForm = false;

  showDetails = false;

  isEdit = false;

  selectedCarId = 0;

  selectedCar: any = null;

  constructor(
    private carsService: CarsService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.carForm = this.fb.group({
      brand: ['', Validators.required],

      model: ['', Validators.required],

      year: ['', [Validators.required, Validators.min(1980), Validators.max(2030)]],

      plateNumber: ['', Validators.required],

      color: ['', Validators.required],

      mileage: [0, [Validators.required, Validators.min(0)]],
    });

    this.loadCars();
  }

  loadCars(): void {
    this.loading = true;

    this.carsService.getCars().subscribe({
      next: (data) => {
        console.log('Cars Data:', data);

        this.cars = data;

        this.loading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.log('Cars Error:', error);

        this.loading = false;

        this.cdr.detectChanges();
      },
    });
  }

  openAdd(): void {
    this.isEdit = false;

    this.selectedCarId = 0;

    this.carForm.reset({
      mileage: 0,
    });

    this.showForm = true;
  }

  openEdit(car: any): void {
    this.isEdit = true;

    this.selectedCarId = car.id;

    this.carForm.patchValue({
      brand: car.brand,
      model: car.model,
      year: car.year,
      plateNumber: car.plateNumber,
      color: car.color,
      mileage: car.mileage,
    });

    this.showForm = true;
  }


  closeForm(): void {
    this.showForm = false;

    this.isEdit = false;

    this.selectedCarId = 0;

    this.carForm.reset({
      mileage: 0,
    });
  }

  saveCar(): void {
    if (this.carForm.invalid) {
      this.carForm.markAllAsTouched();

      return;
    }

    if (this.isEdit) {
      this.carsService.updateCar(this.selectedCarId, this.carForm.value).subscribe({
        next: () => {
          this.closeForm();

          this.loadCars();
        },

        error: (error) => {
          alert(error.error?.message || 'Failed to update car');
        },
      });

      
    } else {
      this.carsService.addCar(this.carForm.value).subscribe({
        next: () => {
          this.closeForm();

          this.loadCars();
        },

        error: (error) => {
          alert(error.error?.message || 'Failed to add car');
        },
      });
    }
  }

  viewDetails(id: number): void {
    this.carsService.getCar(id).subscribe({
      next: (data) => {
        this.selectedCar = data;

        this.showDetails = true;
      },

      error: (error) => {
        console.log(error);
      },
    });
  }

  closeDetails(): void {
    this.showDetails = false;

    this.selectedCar = null;
  }

  deleteCar(id: number): void {
    const result = confirm('Are you sure you want to delete this car?');

    if (!result) {
      return;
    }

    this.carsService.deleteCar(id).subscribe({
      next: () => {
        this.loadCars();
      },

      error: (error) => {
        alert(error.error?.message || 'Cannot delete this car');
      },
    });
  }
}
