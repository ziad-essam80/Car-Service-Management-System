import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class BookingsService {

  private apiUrl =
    'http://localhost:5235/api/customer/bookings';


  constructor(
    private http: HttpClient
  ) {}


  getBookings() {

    return this.http.get<any[]>(
      this.apiUrl,
      {
        withCredentials: true
      }
    );

  }


  getBooking(id: number) {

    return this.http.get<any>(
      `${this.apiUrl}/${id}`,
      {
        withCredentials: true
      }
    );

  }


  createBooking(data: any) {

    return this.http.post(
      this.apiUrl,
      data,
      {
        withCredentials: true
      }
    );

  }

}