import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../services/dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  stats: any[] = [];
  serviceOverview: any[] = [];
  appointments: any[] = [];
  jobs: any[] = [];
  mechanics: any[] = [];
  inventory = [
    {
      name: 'Tyre 205/55 R16',
      stock: 'Only 4 units left',
      icon: 'bi bi-circle',
      status: 'Low Stock',
      statusClass: 'low',
    },
    {
      name: 'Engine Oil 5W-30',
      stock: 'Only 3 units left',
      icon: 'bi bi-droplet-fill',
      status: 'Reorder Soon',
      statusClass: 'reorder',
    },
  ];

  constructor(
    private dashboardService: DashboardService,
    private cdr: ChangeDetectorRef,
  ) {}

  ngOnInit(): void {
    this.loadStats();
    this.loadServiceOverview();
    this.loadAppointments();
    this.loadServiceJobs();
    this.loadMechanicsPerformance();
  }


  loadStats(): void {
    this.dashboardService.getStats().subscribe({
      next: (data) => {
        this.stats = [
          {
            title: 'Total Vehicles',
            value: String(data.totalCars),
            image: 'images/car-icon.png',
          },
          {
            title: 'Total Services',
            value: String(data.totalServices),
            image: 'images/service-icon.png',
          },
          {
            title: 'Total Bookings',
            value: String(data.totalBookings),
            image: 'images/calendar-icon.png',
          },
          {
            title: 'Total Invoices',
            value: String(data.totalInvoices),
            image: 'images/wallet-icon.png',
          },
        ];

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Stats Error:', error);
      },
    });
  }

  loadServiceOverview(): void {
    this.dashboardService.getServiceOverview().subscribe({
      next: (data) => {
        const total = data.reduce((sum: number, service: any) => sum + service.value, 0);
        const colors = ['#ef3d50', '#6cbd55', '#ffad32', '#1599dc', '#ff773c'];
        const backgrounds = ['#ffe9ec', '#eaf8e7', '#fff2df', '#e6f3ff', '#ffebe2'];
        this.serviceOverview = data.map((service: any, index: number) => {
          let percentage = 0;
          if (total > 0) {
            percentage = Math.round((service.value / total) * 100);
          }
          return {
            name: service.name,
            value: service.value,
            percentage: percentage + '%',
            color: colors[index % colors.length],
            background: backgrounds[index % backgrounds.length],
          };
        });

        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Service Overview Error:', error);
      },
    });
  }


  loadAppointments(): void {
    this.dashboardService.getAppointments().subscribe({
      next: (data) => {
        this.appointments = data.map((item: any) => {
          return {
            time: this.formatTime(item.time),
            image: '/images/car-icon.png',
            vehicle: item.vehicle,
            customer: item.customer,
            service: item.service,
            status: item.status,
            statusClass: this.getStatusClass(item.status),
          };
        });

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Appointments Error:', error);
      },
    });
  }

  loadServiceJobs(): void {
    this.dashboardService.getServiceJobs().subscribe({
      next: (data) => {
        this.jobs = data.map((item: any, index: number) => {
          return {
            image: '/images/car-icon.png',
            vehicle: item.vehicle,
            customer: item.customer,
            service: item.service,
            mechanic: item.mechanic,
            mechanicImage: `/images/mechanic${(index % 5) + 1}.jpg`,
            status: item.status,
            statusClass: this.getStatusClass(item.status),
          };
        });

        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Service Jobs Error:', error);
      },
    });
  }

  loadMechanicsPerformance(): void {
    this.dashboardService.getMechanicsPerformance().subscribe({
      next: (data) => {
        let maxJobs = 1;

        if (data.length > 0) {
          maxJobs = Math.max(...data.map((mechanic: any) => mechanic.value));
        }

        this.mechanics = data.map((mechanic: any, index: number) => {
          return {
            name: mechanic.name,

            value: mechanic.value,

            percentage: Math.round((mechanic.value / maxJobs) * 100),

            image: `/images/mechanic${(index % 5) + 1}.jpg`,
          };
        });

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Mechanics Performance Error:', error);
      },
    });
  }

  getStatusClass(status: string): string {
    if (!status) {
      return '';
    }

    const value = status.toLowerCase();

    if (value.includes('progress')) {
      return 'progress';
    }

    if (value.includes('pending')) {
      return 'pending';
    }

    if (value.includes('completed')) {
      return 'completed';
    }

    return '';
  }

  formatTime(time: string): string {
    if (!time) {
      return '';
    }

    const parts = time.split(':');

    let hour = Number(parts[0]);

    const minute = parts[1];

    const period = hour >= 12 ? 'PM' : 'AM';

    hour = hour % 12;

    if (hour === 0) {
      hour = 12;
    }

    return `${hour.toString().padStart(2, '0')}:${minute} ${period}`;
  }
}
