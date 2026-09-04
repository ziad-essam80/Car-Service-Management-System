import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CustomersService {
  private apiUrl = 'http://localhost:5235/api/customers';

  constructor(private http: HttpClient) {}

  getCustomers(): Observable<any[]> 
  {
    return this.http.get<any[]>(this.apiUrl);
  }

  getTotalCustomers(): Observable<any> 
  {
    return this.http.get<any>(`${this.apiUrl}/total-count`);
  }

  getActiveCustomers(): Observable<any> 
  {
    return this.http.get<any>(`${this.apiUrl}/active-count`);
  }

  getNewCustomers(): Observable<any> 
  {

    return this.http.get<any>(`${this.apiUrl}/new-count`);
  }

  getRegisteredVehicles(): Observable<any>
   {
    return this.http.get<any>(`${this.apiUrl}/registered-vehicles`);
  }

  deleteCustomer(id: number, force: boolean = false): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}?force=${force}`);
  }

  updateCustomer(id: number, customer: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, customer);
  }

  getCustomer(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
}
