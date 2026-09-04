import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ServicesApiService } from '../services/services-api';

@Component({
  selector: 'app-services',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './services.html',
  styleUrl: './services.css',
})
export class Services implements OnInit {
  services: any[] = [];
  searchText = '';
  modalMode: 'add' | 'edit' | null = null;
  selectedService: any = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    estimatedTime: 60,
    imageUrl: '',
    isActive: true,
  };

  constructor(
    private servicesApi: ServicesApiService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadServices();
  }
  loadServices(): void {
    this.servicesApi.getServices().subscribe({
      next: (data) => {
        this.services = data;

        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Services Error:', error);
      },
    });
  }
  get filteredServices(): any[] {
    if (!this.searchText) {
      return this.services;
    }
    const search = this.searchText.toLowerCase().trim();
    return this.services.filter(
      (service) =>
        service.name?.toLowerCase().includes(search) ||
        service.description?.toLowerCase().includes(search),
    );
  }

  openAddModal(): void {
    this.selectedService = {
      id: 0,
      name: '',
      description: '',
      price: 0,
      estimatedTime: 60,
      imageUrl: '',
      isActive: true,
      status: 'Active',
    };
    this.modalMode = 'add';
  }
  openEditModal(service: any): void {
    this.selectedService = {
      ...service,
    };
    this.modalMode = 'edit';
  }

  closeModal(): void {
    this.modalMode = null;
  }
  saveService(): void {
    if (!this.selectedService.name?.trim()) {
      alert('Please enter service name');
      return;
    }

    if (Number(this.selectedService.price) <= 0) {
      alert('Price must be greater than 0');
      return;
    }
    if (Number(this.selectedService.estimatedTime) <= 0) {
      alert('Estimated time must be greater than 0');

      return;
    }

    if (this.modalMode === 'add') {
      this.addService();
    } else {
      this.updateService();
    }
  }

  addService(): void {
    const service = {
      name: this.selectedService.name,

      description: this.selectedService.description,

      price: Number(this.selectedService.price),

      estimatedTime: Number(this.selectedService.estimatedTime) || 60,

      imageUrl: this.selectedService.imageUrl,

      isActive: true,

      status: 'Active',
    };

    console.log('Service Data:', service);

    this.servicesApi.addService(service).subscribe({
      next: (response) => {
        console.log('Service Added:', response);

        this.closeModal();

        this.loadServices();
      },

      error: (error) => {
        console.error('Add Service Error:', error);

        console.error('Backend Response:', error.error);
      },
    });
  }

  updateService(): void {
    const service = {
      name: this.selectedService.name,

      description: this.selectedService.description,

      price: Number(this.selectedService.price),

      estimatedTime: Number(this.selectedService.estimatedTime),

      imageUrl: this.selectedService.imageUrl,

      isActive: this.selectedService.isActive,
    };

    this.servicesApi.updateService(this.selectedService.id, service).subscribe({
      next: () => {
        this.closeModal();

        this.loadServices();
      },
      error: (error) => {
        console.error('Update Service Error:', error);
      },
    });
  }

  toggleStatus(service: any): void {
    this.servicesApi.toggleStatus(service.id).subscribe({
      next: (data) => {
        service.isActive = data.isActive;

        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Toggle Status Error:', error);
      },
    });
  }

  deleteService(id: number): void {
    const result = confirm('Are you sure you want to delete this service?');
    if (!result) {
      return;
    }
    this.servicesApi.deleteService(id).subscribe({
      next: () => {
        this.loadServices();
      },
      error: (error) => {
        if (error.error?.message) {
          alert(error.error.message);
        } else {
          alert('Failed to delete service');
        }
      },
    });
  }
}
