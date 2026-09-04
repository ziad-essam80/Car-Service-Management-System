import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ServicesService {

  private apiUrl = 'http://localhost:5235/api/customer/services';

  constructor(private http: HttpClient) {}

  getServices(): Observable<any[]> {
    return this.http.get<any[]>(
      this.apiUrl,
      {
        withCredentials: true
      }
    );
  }

  getService(id: number): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/${id}`,
      {
        withCredentials: true
      }
    );
  }

}