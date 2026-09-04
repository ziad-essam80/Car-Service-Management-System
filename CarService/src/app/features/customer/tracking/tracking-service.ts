import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TrackingService {

  private apiUrl =
    'http://localhost:5235/api/customer/bookings';

  constructor(private http: HttpClient) {}


  getTracking(
    bookingId: number
  ): Observable<any> {

    return this.http.get(
      `${this.apiUrl}/${bookingId}/tracking`,
      {
        withCredentials: true
      }
    );

  }

}