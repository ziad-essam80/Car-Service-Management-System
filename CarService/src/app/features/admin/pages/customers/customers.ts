import { Component, OnInit, ChangeDetectorRef } from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { CustomersService } from '../services/customers';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './customers.html',
  styleUrl: './customers.css',
})
export class Customers implements OnInit {
  modalMode: 'view' | 'edit' | null = null;
  selectedCustomer: any = null;
  totalCustomers = 0;
  activeCustomers = 0;
  newCustomers = 0;
  registeredVehicles = 0;
  carsPerCustomer = 0;
  customers: any[] = [];
  searchText = '';

  constructor(
    private customersService: CustomersService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadTotalCustomers();
    this.loadActiveCustomers();
    this.loadNewCustomers();
    this.loadRegisteredVehicles();
    this.loadCustomers();
  }

  loadTotalCustomers(): void {
    this.customersService.getTotalCustomers().subscribe({
      next: (data) => {
        this.totalCustomers = data.totalCustomers;

        this.cdr.detectChanges();
      },
    });
  }

  loadActiveCustomers(): void {
    this.customersService.getActiveCustomers().subscribe({
      next: (data) => {
        this.activeCustomers = data.activeCustomers;

        this.cdr.detectChanges();
      },
    });
  }

  loadNewCustomers(): void {
    this.customersService.getNewCustomers().subscribe({
      next: (data) => {
        this.newCustomers = data.newCustomers;

        this.cdr.detectChanges();
      },
    });
  }

  loadRegisteredVehicles(): void {
    this.customersService.getRegisteredVehicles().subscribe({
      next: (data) => {
        this.registeredVehicles = data.registeredVehicles;

        this.carsPerCustomer = data.carsPerCustomer;

        this.cdr.detectChanges();
      },
    });
  }

  loadCustomers(): void {
    this.customersService.getCustomers().subscribe({
      next: (data) => {
        this.customers = data.map((customer) => {
          return {
            ...customer,

            image: '/images/customer.png',
          };
        });

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Customers Error:', error);
      },
    });
  }

  get filteredCustomers(): any[] {
    if (!this.searchText) {
      return this.customers;
    }

    const search = this.searchText.toLowerCase().trim();

    return this.customers.filter(
      (customer) =>
        customer.name?.toLowerCase().includes(search) ||
        customer.email?.toLowerCase().includes(search) ||
        customer.phone?.toLowerCase().includes(search),
    );
  }

  deleteCustomer(id: number): void {
    const firstConfirm = confirm('Are you sure you want to delete this customer?');

    if (!firstConfirm) {
      return;
    }

    const finalConfirm = confirm(
      'WARNING: This will permanently delete the customer, vehicles, bookings, service jobs and invoices. This action cannot be undone. Continue?',
    );

    if (!finalConfirm) {
      return;
    }

    this.customersService.deleteCustomer(id, true).subscribe({
      next: (response) => {
        alert(response.message);

        this.loadCustomers();

        this.loadTotalCustomers();

        this.loadActiveCustomers();

        this.loadNewCustomers();

        this.loadRegisteredVehicles();
      },

      error: (error) => {
        if (error.error?.message) {
          alert(error.error.message);
        } else {
          alert('Failed to delete customer.');
        }
      },
    });
  }

  getActivePercentage(): number {
    if (this.totalCustomers === 0) {
      return 0;
    }

    return Math.round((this.activeCustomers / this.totalCustomers) * 100);
  }

  editCustomer(id: number): void {
    this.customersService.getCustomer(id).subscribe({
      next: (data) => {
        this.selectedCustomer = data;

        this.modalMode = 'edit';

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Get Customer Error:', error);
      },
    });
  }

  viewCustomer(customer: any): void {
    this.selectedCustomer = customer;

    this.modalMode = 'view';
  }
  closeModal(): void {
    this.modalMode = null;

    this.selectedCustomer = null;
  }

  saveCustomer(): void {
    if (!this.selectedCustomer) {
      return;
    }

    this.customersService
      .updateCustomer(this.selectedCustomer.id, this.selectedCustomer)
      .subscribe({
        next: () => {
          this.closeModal();

          this.loadCustomers();
        },

        error: (error) => {
          console.error('Update Customer Error:', error);

          alert('Failed to update customer');
        },
      });
  }
}
