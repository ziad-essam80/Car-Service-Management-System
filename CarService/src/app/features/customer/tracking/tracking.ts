import {
  Component,
  OnInit,
  ChangeDetectorRef
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { TrackingService } from './tracking-service';

@Component({
  selector: 'app-tracking',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './tracking.html',
  styleUrl: './tracking.css'
})
export class Tracking implements OnInit {

  bookingId = 0;

  tracking: any = null;

  loading = true;

  errorMessage = '';


  constructor(
    private route: ActivatedRoute,
    private trackingService: TrackingService,
    private cdr: ChangeDetectorRef
  ) {}


  ngOnInit(): void {

    this.bookingId = Number(
      this.route.snapshot.paramMap.get('id')
    );

    this.loadTracking();

  }


  loadTracking(): void {

    this.loading = true;

    this.errorMessage = '';


    this.trackingService
      .getTracking(this.bookingId)
      .subscribe({

        next: (data) => {

          console.log(
            'Tracking API Response:',
            data
          );


          this.tracking = data;

          this.loading = false;


          this.cdr.detectChanges();

        },


        error: (error) => {

          console.error(
            'Tracking API Error:',
            error
          );


          this.tracking = null;

          this.errorMessage =
            'Unable to load service tracking.';


          this.loading = false;


          this.cdr.detectChanges();

        }

      });

  }

}