import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  private apiUrl = 'http://localhost:5235/api/dashboard';

  constructor(private http: HttpClient) {}

  getStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/stats`);
  }

  getServiceOverview(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/service-overview`);
  }

  getAppointments(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/appointments`);
  }

  getServiceJobs(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/service-jobs`);
  }

  getMechanicsPerformance(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/mechanics-performance`);
  }
}
