import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private apiUrl =
    'http://localhost:5235/api/customer-auth/dashboard';

  constructor(private http: HttpClient) {}


  getDashboard(): Observable<any> {

    return this.http.get(
      this.apiUrl,
      {
        withCredentials: true
      }
    );

  }

}