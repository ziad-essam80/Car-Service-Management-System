import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  private apiUrl =
    'http://localhost:5235/api/customer-auth/profile';


  constructor(
    private http: HttpClient
  ) {}


  getProfile() {

    return this.http.get<any>(
      this.apiUrl,
      {
        withCredentials: true
      }
    );

  }


  updateProfile(data: any) {

    return this.http.put(
      this.apiUrl,
      data,
      {
        withCredentials: true
      }
    );

  }

}