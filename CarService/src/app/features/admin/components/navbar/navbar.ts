import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../auth/services/auth';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class NavbarComponent implements OnInit {
  adminName = '';
  adminRole = '';
  adminEmail = '';
  adminInitials = 'AD';

  constructor(
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadAdmin();
  }



  loadAdmin(): void {
    this.authService.getCurrentAdmin().subscribe({
      next: (data) => {
        this.adminName = data.fullName;
        this.adminRole = data.role;
        this.adminEmail = data.email;
        this.adminInitials = this.getInitials(data.fullName);
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Admin Error:', error);
      },
    });
  }
  getInitials(name: string): string {
    if (!name) {
      return 'AD';
    }
    const parts = name.trim().split(' ');

    if (parts.length === 1) {
      return parts[0].substring(0, 2).toUpperCase();
    }

    return (parts[0][0] + parts[1][0]).toUpperCase();
  }
}
