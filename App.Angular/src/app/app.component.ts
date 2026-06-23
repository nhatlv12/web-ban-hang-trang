import { Component, signal } from '@angular/core';
import { Router, RouterOutlet, RouterLink, RouterLinkActive, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { ButtonModule } from 'primeng/button';
import { ToolbarModule } from 'primeng/toolbar';
import { AvatarModule } from 'primeng/avatar';
import { ToastModule } from 'primeng/toast';
import { MenuModule } from 'primeng/menu';
import { RippleModule } from 'primeng/ripple';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { BadgeModule } from 'primeng/badge';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService, MenuItem, ConfirmationService } from 'primeng/api';
import { AuthService } from './core/services/auth.service';

// Lucide Icons
import {
  LucideLayoutDashboard,
  LucideTruck,
  LucideUsers,
  LucideLayoutGrid,
  LucidePackage,
  LucideWarehouse,
  LucideShoppingCart,
  LucidePanelLeft,
  LucideAlignJustify,
  LucideMoon,
  LucideSun,
  LucideBell,
  LucideChevronUp,
  LucideChevronDown,
  LucideShoppingBag,
  LucideGrid3x3,
} from '@lucide/angular';

interface NavItem {
  label: string;
  iconSelector: string;
  route: string;
  badge?: number;
}

interface NavGroup {
  title: string;
  items: NavItem[];
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet, RouterLink, RouterLinkActive,
    ButtonModule, ToolbarModule, AvatarModule,
    ToastModule, MenuModule, RippleModule,
    ConfirmDialogModule, BadgeModule, TooltipModule,
    // Lucide Icon Components
    LucideLayoutDashboard, LucideTruck, LucideUsers, LucideLayoutGrid,
    LucidePackage, LucideWarehouse, LucideShoppingCart,
    LucidePanelLeft, LucideAlignJustify,
    LucideMoon, LucideSun, LucideBell,
    LucideChevronUp, LucideChevronDown,
    LucideShoppingBag, LucideGrid3x3,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class App {
  protected readonly title = signal('Trang Store');
  protected readonly isDarkMode = signal(false);
  protected readonly showLayout = signal(true);
  protected readonly sidebarCollapsed = signal(false);
  protected readonly mobileMenuOpen = signal(false);
  protected readonly appMenuOpen = signal(false);

  protected readonly appItems = [
    { label: 'Dashboard', icon: 'layout-dashboard', route: '/', color: '#10b981' },
    { label: 'Nhà cung cấp', icon: 'truck', route: '/providers', color: '#3b82f6' },
    { label: 'Khách hàng', icon: 'users', route: '/customers', color: '#8b5cf6' },
    { label: 'Danh mục', icon: 'layout-grid', route: '/categories', color: '#f59e0b' },
    { label: 'Sản phẩm', icon: 'package', route: '/products', color: '#ef4444' },
    { label: 'Kho hàng', icon: 'warehouse', route: '/warehouses', color: '#06b6d4' },
    { label: 'Đơn hàng', icon: 'shopping-cart', route: '/orders', color: '#ec4899' },
  ];

  protected readonly navGroups: NavGroup[] = [
    {
      title: 'TỔNG QUAN',
      items: [
        { label: 'Dashboard', iconSelector: 'layout-dashboard', route: '/' },
      ]
    },
    {
      title: 'QUẢN LÝ',
      items: [
        { label: 'Nhà cung cấp', iconSelector: 'truck', route: '/providers' },
        { label: 'Khách hàng', iconSelector: 'users', route: '/customers' },
        { label: 'Danh mục', iconSelector: 'layout-grid', route: '/categories' },
        { label: 'Sản phẩm', iconSelector: 'package', route: '/products' },
      ]
    },
    {
      title: 'KHO & BÁN HÀNG',
      items: [
        { label: 'Kho hàng', iconSelector: 'warehouse', route: '/warehouses' },
        { label: 'Đơn hàng', iconSelector: 'shopping-cart', route: '/orders', badge: 3 },
      ]
    }
  ];

  protected readonly userMenuItems: MenuItem[] = [
    { label: 'Hồ sơ cá nhân', icon: 'pi pi-user', command: () => this.messageService.add({ severity: 'info', summary: 'Thông báo', detail: 'Tính năng đang phát triển.', life: 2000 }) },
    { label: 'Cài đặt', icon: 'pi pi-cog', command: () => this.messageService.add({ severity: 'info', summary: 'Thông báo', detail: 'Tính năng đang phát triển.', life: 2000 }) },
    { separator: true },
    { label: 'Đăng xuất', icon: 'pi pi-sign-out', command: () => this.logout() }
  ];

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
    document.documentElement.classList.toggle('app-dark', this.isDarkMode());
  }

  toggleSidebar(): void {
    if (window.innerWidth <= 768) {
      this.mobileMenuOpen.update(v => !v);
    } else {
      this.sidebarCollapsed.update(v => !v);
    }
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen.set(false);
  }

  onNavClick(): void {
    if (window.innerWidth <= 768) {
      this.mobileMenuOpen.set(false);
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
    this.messageService.add({ severity: 'info', summary: 'Đã đăng xuất', detail: 'Bạn đã đăng xuất thành công.', life: 3000 });
  }
}
