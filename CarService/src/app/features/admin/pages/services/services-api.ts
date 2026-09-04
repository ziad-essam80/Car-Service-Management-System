import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ServicesApiService {
  private apiUrl = 'http://localhost:5235/api/services';

  constructor(private http: HttpClient) {}

  getServices(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getAvailableServices(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/available`);
  }

  getService(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  addService(service: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, service);
  }

  updateService(id: number, service: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, service);
  }

  toggleStatus(id: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}/toggle-status`, {});
  }

  deleteService(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
