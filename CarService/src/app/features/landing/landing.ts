import { ChangeDetectorRef, Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ChatService, ChatResponse } from '../service/chat.service';

@Component({
  selector: 'app-landing',
  standalone: true,

  imports: [CommonModule, FormsModule],

  templateUrl: './landing.html',
  styleUrl: './landing.css',
})
export class Landing {
  showChat = false;
  userMessage = '';
  loading = false;
  messages: any[] = [
    {
      sender: 'bot',
      text: 'Hello! 👋 I am the CarService Assistant. How can I help you today?',
    },
  ];

constructor(
  private router: Router,
  private chatService: ChatService,
  private cdr: ChangeDetectorRef
) {}

  // =========================
  // NAVIGATION
  // =========================

  goToServices(): void {
    this.router.navigate(['/services']);
  }

  goToLogin(): void {
    this.router.navigate(['/customer/login']);
  }

  getStarted(): void {
    this.router.navigate(['/customer/register']);
  }

  // =========================
  // CHAT
  // =========================

  toggleChat(): void {
    this.showChat = !this.showChat;
  }

  closeChat(): void {
    this.showChat = false;
  }

  useSuggestion(question: string): void {
    this.userMessage = question;

    this.sendMessage();
  }

sendMessage(): void {

  const message = this.userMessage.trim();

  if (!message || this.loading) {
    return;
  }


  // Add user message
  this.messages.push({
    sender: 'user',
    text: message
  });


  this.userMessage = '';

  this.loading = true;


  this.chatService
    .sendMessage(message)
    .subscribe({

      next: (response) => {

        console.log('Chat response:', response);


        this.messages.push({
          sender: 'bot',
          text: response.reply,
          type: response.type,
          data: response.data
        });


        this.loading = false;


        // Important
        this.cdr.detectChanges();

      },


      error: (error) => {

        console.error(
          'Chat API Error:',
          error
        );


        this.messages.push({
          sender: 'bot',
          text:
            'Sorry, something went wrong. Please try again.'
        });


        this.loading = false;


        // Important
        this.cdr.detectChanges();

      }

    });

}
}
