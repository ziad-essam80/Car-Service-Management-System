import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ChatRequest {
  message: string;
}

export interface ChatResponse {
  reply: string;
  type?: string;
  data?: any[];
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  private readonly apiUrl =
    'http://localhost:5235/api/public-chat';

  constructor(private http: HttpClient) {}

  sendMessage(message: string): Observable<ChatResponse> {

    const body: ChatRequest = {
      message: message
    };

    return this.http.post<ChatResponse>(
      this.apiUrl,
      body
    );
  }
}