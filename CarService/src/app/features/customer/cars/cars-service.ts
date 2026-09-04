import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CarsService {

  private apiUrl =
    'http://localhost:5235/api/customer/cars';

  constructor(private http: HttpClient) {}


  getCars(): Observable<any> {

    return this.http.get(
      this.apiUrl,
      {
        withCredentials: true
      }
    );

  }


  getCar(id: number): Observable<any> {

    return this.http.get(
      `${this.apiUrl}/${id}`,
      {
        withCredentials: true
      }
    );

  }


  addCar(data: any): Observable<any> {

    return this.http.post(
      this.apiUrl,
      data,
      {
        withCredentials: true
      }
    );

  }


  updateCar(
    id: number,
    data: any
  ): Observable<any> {

    return this.http.put(
      `${this.apiUrl}/${id}`,
      data,
      {
        withCredentials: true
      }
    );

  }


  deleteCar(id: number): Observable<any> {

    return this.http.delete(
      `${this.apiUrl}/${id}`,
      {
        withCredentials: true
      }
    );

  }

}