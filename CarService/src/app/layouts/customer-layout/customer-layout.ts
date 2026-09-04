import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../features/customer/auth/auth';

@Component({
  selector: 'app-customer-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './customer-layout.html',
  styleUrl: './customer-layout.css',
})
export class CustomerLayout implements OnInit {
  customer: any = null;

  sidebarOpen = true;

  constructor(
    private authService: AuthService,
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadCustomer();
  }

  loadCustomer(): void {
    this.authService.me().subscribe({
      next: (data) => {
        console.log('Current Customer:', data);

        this.customer = data;

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.log('Customer Error:', error);
      },
    });
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  getInitials(): string {
    if (!this.customer?.fullName) {
      return 'CU';
    }

    const names = this.customer.fullName.trim().split(' ');

    if (names.length === 1) {
      return names[0].substring(0, 2).toUpperCase();
    }

    return (names[0][0] + names[names.length - 1][0]).toUpperCase();
  }

  logout(): void {
    localStorage.removeItem('CarServiceAdmin');
    
    this.authService.logout().subscribe({
      
      next: () => {
        this.router.navigate(['/customer/login']);
      },

      error: (error) => {
        console.log(error);
      },
    });
  }
}
