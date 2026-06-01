import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(m => m.Login),
    title: 'Đăng nhập - Web Bán Hàng Trang'
  },
  {
    path: '',
    loadComponent: () => import('./pages/home/home.component').then(m => m.Home),
    canActivate: [authGuard],
    title: 'Trang chủ - Web Bán Hàng Trang'
  },
  {
    path: 'forbidden',
    loadComponent: () => import('./pages/forbidden/forbidden.component').then(m => m.Forbidden),
    title: '403 - Không có quyền truy cập'
  },
  {
    path: '**',
    loadComponent: () => import('./pages/not-found/not-found.component').then(m => m.NotFound),
    title: '404 - Không tìm thấy trang'
  }
];
