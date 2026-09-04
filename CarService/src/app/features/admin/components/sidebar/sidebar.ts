import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';




@Component({
  imports: [RouterLinkActive, RouterLink],
  selector: 'app-sidebar',
  styleUrl: './sidebar.css',
  templateUrl: './sidebar.html',
})
export class Sidebar {
  constructor(private router: Router) {}

  logout(): void {
    localStorage.removeItem('role');

    this.router.navigate(['/']);
  }
}
