import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  email = '';
  password = '';
  errorMessage = '';
  loading = false;

  authService = inject(AuthService);
  router = inject(Router);

  
  login(): void {
    if (!this.email || !this.password) {
      this.errorMessage = 'Please enter email and password';

      return;
    }

    this.loading = true;
    this.errorMessage = '';


    this.authService.login(this.email, this.password).subscribe({
      next: (data) => {
        // console.log(data);

        localStorage.setItem('role', data.admin.role);
        this.loading = false;
        this.router.navigate(['/admin/dashboard']);
      },




      error: (error) => {
        this.loading = false;
        this.errorMessage = error.error?.message || 'Invalid email or password';
      },
    });
  }
}
