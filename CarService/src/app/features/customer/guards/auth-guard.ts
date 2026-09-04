import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { catchError, map, of } from 'rxjs';
import { AuthService } from '../auth/auth';

export const authGuard: CanActivateFn = (route, state) => {

  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.me().pipe(

    map(() => {
      return true;
    }),

    catchError(() => {

      return of(
        router.createUrlTree([
          '/customer/login'
        ])
      );

    })

  );

};