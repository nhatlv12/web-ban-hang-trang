import { Component, signal } from '@angular/core';
import { Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { AvatarModule } from 'primeng/avatar';
import { ToastModule } from 'primeng/toast';
import { MenuModule } from 'primeng/menu';
import { MessageService, MenuItem } from 'primeng/api';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    ButtonModule,
    ToolbarModule,
    AvatarModule,
    ToastModule,
    MenuModule
  ],
  providers: [MessageService],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class App {
  protected readonly title = signal('Web Bán Hàng Trang');
  protected readonly isDarkMode = signal(false);
  protected readonly showLayout = signal(true);

  protected readonly userMenuItems: MenuItem[] = [
    { label: 'Hồ sơ', icon: 'pi pi-user' },
    { label: 'Cài đặt', icon: 'pi pi-cog' },
    { separator: true },
    { label: 'Đăng xuất', icon: 'pi pi-sign-out', command: () => this.logout() }
  ];

  /** Các route không hiển thị toolbar/layout */
  private readonly noLayoutRoutes = ['/login', '/forbidden'];

  constructor(
    protected authService: AuthService,
    private router: Router,
    private messageService: MessageService
  ) {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        const url = event.urlAfterRedirects;
        this.showLayout.set(
          !this.noLayoutRoutes.some(r => url.startsWith(r)) && !url.startsWith('/404')
        );
      });
  }

  toggleDarkMode(): void {
    this.isDarkMode.update(v => !v);
    if (this.isDarkMode()) {
      document.documentElement.classList.add('app-dark');
    } else {
      document.documentElement.classList.remove('app-dark');
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
    this.messageService.add({
      severity: 'info',
      summary: 'Đã đăng xuất',
      detail: 'Bạn đã đăng xuất thành công.',
      life: 3000
    });
  }
}
