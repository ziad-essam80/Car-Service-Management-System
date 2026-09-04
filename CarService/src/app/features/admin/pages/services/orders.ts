import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class OrdersService {
  private apiUrl = 'http://localhost:5235/api/orders';

  constructor(private http: HttpClient) {}

  getServiceJobs(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/details`);
  }

  nextStatus(id: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}/next-status`, {});
  }
}
