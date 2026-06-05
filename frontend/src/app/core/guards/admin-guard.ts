import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Role } from '../../shared/models/common.enums';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService)
  const router = inject(Router)

  const user = authService.currentUser();

  if (!user?.isAdmin()) {
    router.navigate(['/equipment']);
    return false;
  }

  return true;
};
