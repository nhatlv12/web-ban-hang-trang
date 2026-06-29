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
    title: 'Dashboard - Web Bán Hàng Trang'
  },
  {
    path: 'providers',
    loadComponent: () => import('./pages/providers/providers.component').then(m => m.Providers),
    canActivate: [authGuard],
    title: 'Nhà cung cấp - Web Bán Hàng Trang'
  },
  {
    path: 'customers',
    loadComponent: () => import('./pages/customers/customers.component').then(m => m.Customers),
    canActivate: [authGuard],
    title: 'Khách hàng - Web Bán Hàng Trang'
  },
  {
    path: 'categories',
    loadComponent: () => import('./pages/categories/categories.component').then(m => m.Categories),
    canActivate: [authGuard],
    title: 'Danh mục - Web Bán Hàng Trang'
  },
  {
    path: 'products',
    loadComponent: () => import('./pages/products/products.component').then(m => m.Products),
    canActivate: [authGuard],
    title: 'Sản phẩm - Web Bán Hàng Trang'
  },
  {
    path: 'warehouses',
    loadComponent: () => import('./pages/warehouses/warehouses.component').then(m => m.WareHouses),
    canActivate: [authGuard],
    title: 'Kho hàng - Web Bán Hàng Trang'
  },
  {
    path: 'import-orders',
    loadComponent: () => import('./pages/import-orders/import-orders.component').then(m => m.ImportOrders),
    canActivate: [authGuard],
    title: 'Nhập hàng - Web Bán Hàng Trang'
  },
  {
    path: 'orders',
    loadComponent: () => import('./pages/orders/orders.component').then(m => m.Orders),
    canActivate: [authGuard],
    title: 'Đơn hàng bán - Web Bán Hàng Trang'
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
