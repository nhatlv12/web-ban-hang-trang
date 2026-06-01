import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.loggedIn()) {
    return true;
  }

  // Chuyển hướng đến trang đăng nhập, lưu URL hiện tại để redirect sau khi login
  router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
