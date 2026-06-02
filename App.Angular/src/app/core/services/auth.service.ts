import { Injectable, signal } from '@angular/core';
import { User } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly currentUser = signal<User | null>(null);
  private readonly isAuthenticated = signal<boolean>(false);

  readonly user = this.currentUser.asReadonly();
  readonly loggedIn = this.isAuthenticated.asReadonly();

  login(username: string, password: string): boolean {
    // TODO: Gọi API đăng nhập thật
    if (username && password) {
      this.currentUser.set({
        id: '1',
        username,
        email: `${username}@example.com`,
        fullName: 'Người dùng',
        role: 'user'
      });
      this.isAuthenticated.set(true);
      return true;
    }
    return false;
  }

  logout(): void {
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
  }

  getToken(): string | null {
    // TODO: Lấy token từ localStorage hoặc sessionStorage
    return localStorage.getItem('auth_token');
  }

  setToken(token: string): void {
    localStorage.setItem('auth_token', token);
  }

  removeToken(): void {
    localStorage.removeItem('auth_token');
  }
}
