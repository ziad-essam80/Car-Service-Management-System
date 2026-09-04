import { Component } from '@angular/core';
import { NavbarComponent } from '../../features/admin/components/navbar/navbar';
import { RouterOutlet } from '@angular/router';
import { Sidebar } from '../../features/admin/components/sidebar/sidebar';

@Component({
  imports: [NavbarComponent, RouterOutlet,Sidebar],
  selector: 'app-admin-layout',
  styleUrl: './admin-layout.css',
  templateUrl: './admin-layout.html',
})
export class AdminLayout {}
