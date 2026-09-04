import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  Router,
  RouterModule
} from '@angular/router';

import { AuthService } from '../auth/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {

  registerForm: FormGroup;

  loading = false;

  errorMessage = '';

  showPassword = false;

  showConfirmPassword = false;


  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {

    this.registerForm = this.fb.group({

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
      ],

      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6)
        ]
      ],

      confirmPassword: [
        '',
        Validators.required
      ]

    });

  }


  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }


  toggleConfirmPassword(): void {
    this.showConfirmPassword =
      !this.showConfirmPassword;
  }


  register(): void {

    if (this.registerForm.invalid) {

      this.registerForm.markAllAsTouched();

      return;

    }


    const data =
      this.registerForm.value;


    if (
      data.password !==
      data.confirmPassword
    ) {

      this.errorMessage =
        'Passwords do not match';

      return;

    }


    this.loading = true;

    this.errorMessage = '';


    this.authService
      .register(data)
      .subscribe({

        next: () => {

          this.loading = false;

          this.router.navigate([
            '/customer/login'
          ]);

        },

        error: (error) => {

          this.loading = false;

          this.errorMessage =
            error.error?.message ||
            'Registration failed';

        }

      });

  }

}