import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { ProfileService } from './profile-service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile implements OnInit {

  profileForm!: FormGroup;

  customer: any = null;

  loading = true;

  saving = false;

  editMode = false;

  successMessage = '';

  errorMessage = '';


  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService
  ) {}


  ngOnInit(): void {

    this.profileForm = this.fb.group({

      fullName: [
        '',
        Validators.required
      ],

      email: [
        '',
        [
          Validators.required,
          Validators.email
        ]
      ],

      phone: [
        '',
        Validators.required
      ]

    });


    this.profileForm.disable();

    this.loadProfile();

  }


  loadProfile(): void {

    this.loading = true;


    this.profileService
      .getProfile()
      .subscribe({

        next: (data) => {

          this.customer = data;

          this.profileForm.patchValue({

            fullName: data.fullName,

            email: data.email,

            phone: data.phone

          });

          this.loading = false;

        },

        error: (error) => {

          console.log(error);

          this.loading = false;

          this.errorMessage =
            'Failed to load profile';

        }

      });

  }


  enableEdit(): void {

    this.editMode = true;

    this.successMessage = '';

    this.errorMessage = '';

    this.profileForm.enable();

  }


  cancelEdit(): void {

    this.editMode = false;

    this.successMessage = '';

    this.errorMessage = '';

    this.profileForm.patchValue({

      fullName: this.customer.fullName,

      email: this.customer.email,

      phone: this.customer.phone

    });

    this.profileForm.disable();

  }


  updateProfile(): void {

    if (this.profileForm.invalid) {

      this.profileForm.markAllAsTouched();

      return;

    }


    this.saving = true;

    this.successMessage = '';

    this.errorMessage = '';


    this.profileService
      .updateProfile(
        this.profileForm.getRawValue()
      )
      .subscribe({

        next: () => {

          this.customer = {
            ...this.customer,
            ...this.profileForm.getRawValue()
          };

          this.saving = false;

          this.editMode = false;

          this.profileForm.disable();

          this.successMessage =
            'Profile updated successfully';

        },

        error: (error) => {

          this.saving = false;

          this.errorMessage =
            error.error?.message ||
            'Failed to update profile';

        }

      });

  }


  getInitials(): string {

    if (!this.customer?.fullName) {
      return 'CU';
    }


    const names =
      this.customer.fullName
        .trim()
        .split(' ');


    if (names.length === 1) {

      return names[0]
        .substring(0, 2)
        .toUpperCase();

    }


    return (
      names[0][0] +
      names[names.length - 1][0]
    ).toUpperCase();

  }

}