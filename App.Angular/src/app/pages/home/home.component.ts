import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { SearchableSelectComponent } from '../../shared/components/searchable-select/searchable-select.component';

// Lucide Icons
import {
  LucideShoppingCart, LucidePackage, LucideUsers, LucideWallet,
  LucideClipboardList, LucideBarChart3,
  LucideTrendingUp, LucideArrowUpRight,
  LucideDownload, LucideUpload,
} from '@lucide/angular';

@Component({
  selector: 'app-home',
  imports: [
    ButtonModule, CardModule, ChartModule, SelectModule, TableModule, TagModule,
    FormsModule, RouterLink, SearchableSelectComponent,
    LucideShoppingCart, LucidePackage, LucideUsers, LucideWallet,
    LucideClipboardList, LucideBarChart3,
    LucideTrendingUp, LucideArrowUpRight,
    LucideDownload, LucideUpload,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class Home implements OnInit {
  constructor(protected authService: AuthService) { }

  // Period filter
  periodOptions = [
    { label: 'Hôm nay', value: 'today' },
    { label: '7 ngày', value: '7d' },
    { label: '30 ngày', value: '30d' },
    { label: 'Tháng này', value: 'month' },
    { label: 'Năm nay', value: 'year' },
  ];
  selectedPeriod = 'month';

  // Revenue chart
  revenueChartData: any;
  revenueChartOptions: any;

  // Import/Export bar chart
  importExportChartData: any;
  importExportChartOptions: any;

  // Category doughnut chart
  categoryChartData: any;
  categoryChartOptions: any;

  // Order status pie
  orderStatusChartData: any;
  orderStatusChartOptions: any;

  // Recent orders
  recentOrders = [
    { code: 'PX-060001', type: 2, customer: 'Nguyễn Văn An', amount: 34990000, status: 3, date: '02/06/2026' },
    { code: 'PN-060001', type: 1, provider: 'Apple Việt Nam', amount: 900000000, status: 3, date: '01/06/2026' },
    { code: 'PX-060002', type: 2, customer: 'Trần Thị Bình', amount: 55980000, status: 1, date: '02/06/2026' },
    { code: 'PX-060003', type: 2, customer: 'Lê Hoàng Cường', amount: 27490000, status: 2, date: '02/06/2026' },
    { code: 'PN-050001', type: 1, provider: 'Samsung', amount: 500000000, status: 4, date: '30/05/2026' },
  ];

  ngOnInit(): void {
    this.initRevenueChart();
    this.initImportExportChart();
    this.initCategoryChart();
    this.initOrderStatusChart();
  }

  formatPrice(val: number): string {
    if (val >= 1_000_000_000) return (val / 1_000_000_000).toFixed(1) + ' tỷ';
    if (val >= 1_000_000) return (val / 1_000_000).toFixed(0) + ' tr';
    return new Intl.NumberFormat('vi-VN').format(val) + 'đ';
  }

  formatFullPrice(val: number): string {
    return new Intl.NumberFormat('vi-VN').format(val) + 'đ';
  }

  getTypeLabel(t: number) { return t === 1 ? 'Nhập' : 'Xuất'; }
  getTypeSeverity(t: number): 'info' | 'warn' { return t === 1 ? 'info' : 'warn'; }
  getStatusLabel(s: number) { return ['Nháp', 'Chờ xử lý', 'Xác nhận', 'Hoàn thành', 'Đã hủy'][s]; }
  getStatusSeverity(s: number): 'secondary' | 'warn' | 'info' | 'success' | 'danger' {
    return (['secondary', 'warn', 'info', 'success', 'danger'] as const)[s];
  }

  private initRevenueChart(): void {
    const months = ['T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10', 'T11', 'T12'];
    this.revenueChartData = {
      labels: months,
      datasets: [
        {
          label: 'Doanh thu bán hàng',
          data: [120, 190, 150, 230, 280, 310, 0, 0, 0, 0, 0, 0].map(v => v * 1_000_000),
          fill: true,
          borderColor: '#3b82f6',
          backgroundColor: 'rgba(59, 130, 246, 0.08)',
          tension: 0.4,
          pointRadius: 4,
          pointHoverRadius: 6,
          pointBackgroundColor: '#3b82f6',
          borderWidth: 2.5,
        },
        {
          label: 'Chi phí nhập hàng',
          data: [80, 130, 100, 170, 200, 230, 0, 0, 0, 0, 0, 0].map(v => v * 1_000_000),
          fill: true,
          borderColor: '#f59e0b',
          backgroundColor: 'rgba(245, 158, 11, 0.08)',
          tension: 0.4,
          pointRadius: 4,
          pointHoverRadius: 6,
          pointBackgroundColor: '#f59e0b',
          borderWidth: 2.5,
        }
      ]
    };
    this.revenueChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'top', labels: { usePointStyle: true, padding: 20, font: { size: 13, weight: '600' } } },
        tooltip: {
          backgroundColor: '#1e293b',
          titleFont: { size: 13 },
          bodyFont: { size: 12 },
          padding: 12,
          cornerRadius: 8,
          callbacks: { label: (ctx: any) => `${ctx.dataset.label}: ${new Intl.NumberFormat('vi-VN').format(ctx.raw)}đ` }
        }
      },
      scales: {
        x: { grid: { display: false }, ticks: { font: { size: 12 } } },
        y: {
          grid: { color: '#f1f5f9' },
          ticks: { font: { size: 11 }, callback: (v: number) => v >= 1e6 ? (v / 1e6) + 'tr' : v }
        }
      }
    };
  }

  private initImportExportChart(): void {
    this.importExportChartData = {
      labels: ['T1', 'T2', 'T3', 'T4', 'T5', 'T6'],
      datasets: [
        {
          label: 'Nhập kho',
          data: [15, 22, 18, 25, 30, 28],
          backgroundColor: '#6366f1',
          borderRadius: 6,
          barPercentage: 0.6,
        },
        {
          label: 'Xuất kho',
          data: [12, 18, 14, 20, 25, 22],
          backgroundColor: '#10b981',
          borderRadius: 6,
          barPercentage: 0.6,
        }
      ]
    };
    this.importExportChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { position: 'top', labels: { usePointStyle: true, padding: 20, font: { size: 13, weight: '600' } } },
        tooltip: { backgroundColor: '#1e293b', padding: 12, cornerRadius: 8 }
      },
      scales: {
        x: { grid: { display: false }, ticks: { font: { size: 12 } } },
        y: { grid: { color: '#f1f5f9' }, ticks: { font: { size: 11 } }, beginAtZero: true }
      }
    };
  }

  private initCategoryChart(): void {
    this.categoryChartData = {
      labels: ['Điện thoại', 'Laptop', 'Tablet', 'Phụ kiện', 'Đồng hồ'],
      datasets: [{
        data: [35, 25, 15, 20, 5],
        backgroundColor: ['#3b82f6', '#8b5cf6', '#f59e0b', '#10b981', '#ec4899'],
        hoverBackgroundColor: ['#2563eb', '#7c3aed', '#d97706', '#059669', '#db2777'],
        borderWidth: 0,
      }]
    };
    this.categoryChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      cutout: '65%',
      plugins: {
        legend: { position: 'bottom', labels: { usePointStyle: true, padding: 16, font: { size: 12, weight: '500' } } },
        tooltip: { backgroundColor: '#1e293b', padding: 12, cornerRadius: 8 }
      }
    };
  }

  private initOrderStatusChart(): void {
    this.orderStatusChartData = {
      labels: ['Hoàn thành', 'Chờ xử lý', 'Đã xác nhận', 'Đã hủy'],
      datasets: [{
        data: [45, 20, 25, 10],
        backgroundColor: ['#10b981', '#f59e0b', '#3b82f6', '#ef4444'],
        hoverBackgroundColor: ['#059669', '#d97706', '#2563eb', '#dc2626'],
        borderWidth: 0,
      }]
    };
    this.orderStatusChartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      cutout: '60%',
      plugins: {
        legend: { position: 'bottom', labels: { usePointStyle: true, padding: 16, font: { size: 12, weight: '500' } } },
        tooltip: { backgroundColor: '#1e293b', padding: 12, cornerRadius: 8 }
      }
    };
  }
}
