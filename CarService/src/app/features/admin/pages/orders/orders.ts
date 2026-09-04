import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { OrdersService } from '../services/orders';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orders.html',
  styleUrl: './orders.css'
})
export class Orders implements OnInit {

  jobs: any[] = [];

  constructor(
    private serviceJobsService: OrdersService,
    private cdr: ChangeDetectorRef
  ) {}
  ngOnInit(): void {
    this.loadJobs();
  }
  loadJobs(): void {
    this.serviceJobsService
      .getServiceJobs()
      .subscribe({
        next: (data) => {
          this.jobs = data;
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('Orders Error:', error);
        }
      });
  }

  moveToNextStatus(job: any): void {
    if (job.status === 'Completed') {
      return;
    }
    this.serviceJobsService
      .nextStatus(job.id)
      .subscribe({
        next: (data) => {
          job.status = data.status;
          this.cdr.detectChanges();
        },
        error: (error) => {
          console.error('Status Error:', error);
        }
      });
  }

  getButtonText(status: string): string {
    if (status === 'Pending') {
      return 'Start Service';
    }
    if (status === 'In Progress') {
      return 'Complete Service';
    }
    return 'Completed';
  }

getStatusClass(status: string): string {

  switch (status) {

    case 'Booked':
      return 'status-booked';

    case 'Confirmed':
      return 'status-confirmed';

    case 'Vehicle Received':
      return 'status-received';

    case 'Inspection':
      return 'status-inspection';

    case 'In Service':
      return 'status-service';

    case 'Quality Check':
      return 'status-quality';

    case 'Ready for Pickup':
      return 'status-pickup';

    case 'Completed':
      return 'status-completed';

    default:
      return '';
  }

}

  getNextStatusText(status: string): string {

  switch (status) {

    case 'Booked':
      return 'Confirmed';

    case 'Confirmed':
      return 'Vehicle Received';

    case 'Vehicle Received':
      return 'Inspection';

    case 'Inspection':
      return 'In Service';

    case 'In Service':
      return 'Quality Check';

    case 'Quality Check':
      return 'Ready for Pickup';

    case 'Ready for Pickup':
      return 'Completed';

    case 'Completed':
      return 'Completed';

    default:
      return 'Next Step';
  }

}

getActionButtonClass(status: string): string {

  switch (status) {

    case 'Booked':
      return 'btn-booked';

    case 'Confirmed':
      return 'btn-confirmed';

    case 'Vehicle Received':
      return 'btn-received';

    case 'Inspection':
      return 'btn-inspection';

    case 'In Service':
      return 'btn-service';

    case 'Quality Check':
      return 'btn-quality';

    case 'Ready for Pickup':
      return 'btn-pickup';

    case 'Completed':
      return 'btn-completed';

    default:
      return '';
  }

}
}