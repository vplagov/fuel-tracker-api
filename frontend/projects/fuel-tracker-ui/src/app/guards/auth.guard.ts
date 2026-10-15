import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map, of } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.currentUser()) {
    return authService.verifySession().pipe(
      map((isValid) => {
        if (isValid) return true;
        router.navigate(['/login']);
        return false;
      }),
    );
  }

  router.navigate(['/login']);
  return of(false);
};
