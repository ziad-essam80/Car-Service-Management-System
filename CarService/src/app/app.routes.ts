import { Routes } from '@angular/router';
import { AdminLayout } from './layouts/admin-layout/admin-layout';
import { Dashboard } from './features/admin/pages/dashboard/dashboard';
import { Services } from './features/admin/pages/myService/services';
import { Login } from './features/admin/auth/login/login';
import { Orders } from './features/admin/pages/orders/orders';
import { Customers } from './features/admin/pages/customers/customers';
import { CustomerLayout } from './layouts/customer-layout/customer-layout';
import { Login as CustomerLogin } from './features/customer/login/login';
import { Register } from './features/customer/register/register';
import { Dashboard as CustomerDashboard } from './features/customer/dashboard/dashboard';
import { Profile } from './features/customer/profile/profile';
import { Cars as CustomerCars } from './features/customer/cars/cars';
import { Services as CustomerServices } from './features/customer/services/services';
import { Bookings } from './features/customer/bookings/bookings';
import { Tracking } from './features/customer/tracking/tracking';
import { authGuard } from './features/customer/guards/auth-guard';
import { BookService } from './features/customer/book-service/book-service';
import { AdminGuard } from './features/admin/guards/admin-guard';
import { Landing } from './features/landing/landing';

export const routes: Routes = [
    {
    path: '',
    component: Landing,
  },
  {
  path: 'admin/login',
  component: Login
  },
  {
    path: 'admin',
    component: AdminLayout,
    canActivate: [AdminGuard],
    children: [
        {
          path: 'admin/login',
          component: Login,
      },
      {
        path: 'dashboard',
        component: Dashboard,
      },
      {
        path: 'customers',
        component: Customers,
      },
      {
        path: 'services',
        component: Services,
      },
      {
        path: 'orders',
        component: Orders,
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
    ],
  },
  {
    path: 'customer/login',
    component: CustomerLogin,
  },
  {
    path: 'customer/register',
    component: Register,
  },
  {
    path: 'customer',
    component: CustomerLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: CustomerDashboard,
      },
      {
        path: 'profile',
        component: Profile,
      },
      {
        path: 'cars',
        component: CustomerCars,
      },
      {
        path: 'services',
        component: CustomerServices,
      },
      {
        path: 'bookings',
        component: Bookings,
      },
      {
        path: 'tracking/:id',
        component: Tracking,
      },
      {
        path: 'history',
        component: History,
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full',
      },
    ],
  },
  {
    path: 'customer',
    component: CustomerLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'services',
        component: CustomerServices,
      },
      {
        path: 'book-service',
        component: BookService,
      },
    ],
  },
];
