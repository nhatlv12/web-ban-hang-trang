import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
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
import { Api } from '../../api/api';
import { apiDashboardGet } from '../../api/functions';

// Lucide Icons
import {
  LucideShoppingCart, LucidePackage, LucideUsers, LucideWallet,
  LucideClipboardList, LucideBarChart3,
  LucideTrendingUp, LucideArrowUpRight, LucideArrowDownRight,
  LucideDownload, LucideUpload,
} from '@lucide/angular';

@Component({
  selector: 'app-home',
  imports: [
    CommonModule,
    ButtonModule, CardModule, ChartModule, SelectModule, TableModule, TagModule,
    FormsModule, RouterLink, SearchableSelectComponent,
    LucideShoppingCart, LucidePackage, LucideUsers, LucideWallet,
    LucideClipboardList, LucideBarChart3,
    LucideTrendingUp, LucideArrowUpRight, LucideArrowDownRight,
    LucideDownload, LucideUpload,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class Home implements OnInit {
  constructor(protected authService: AuthService, private api: Api) { }

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
  recentOrders = signal<any[]>([]);
  stats = signal<any>({
    totalOrders: 0,
    totalProducts: 0,
    totalCustomers: 0,
    totalRevenue: 0,
    totalImportMonth: 0,
    totalExportMonth: 0,
    estimatedProfit: 0,
    newOrdersToday: 0
  });

  ngOnInit(): void {
    this.loadData();
  }

  async loadData() {
    try {
      const res: any = await this.api.invoke(apiDashboardGet, { period: this.selectedPeriod });
      if (res?.data) {
        const d = res.data;
        this.stats.set(d);
        this.recentOrders.set(d.recentOrders || []);
        this.initRevenueChart(d);
        this.initImportExportChart(d);
        this.initCategoryChart(d);
        this.initOrderStatusChart(d);
      }
    } catch (err) {
      console.error(err);
    }
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
  getStatusSeverity(s: number): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    return (['secondary', 'warn', 'info', 'success', 'danger'] as const)[s];
  }

  onPeriodChange(period: string) {
    this.selectedPeriod = period;
    this.loadData();
  }

  getAbs(val: number): number {
    return Math.abs(val || 0);
  }

  private initRevenueChart(data: any): void {
    const months = ['T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10', 'T11', 'T12'];
    this.revenueChartData = {
      labels: months,
      datasets: [
        {
          label: 'Doanh thu bán hàng',
          data: data.revenueByMonth || [],
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
          data: data.costByMonth || [],
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

  private initImportExportChart(data: any): void {
    this.importExportChartData = {
      labels: data.importExportLabels || [],
      datasets: [
        {
          label: 'Nhập kho',
          data: data.importCounts || [],
          backgroundColor: '#6366f1',
          borderRadius: 6,
          barPercentage: 0.6,
        },
        {
          label: 'Xuất kho',
          data: data.exportCounts || [],
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

  private initCategoryChart(data: any): void {
    this.categoryChartData = {
      labels: data.categoryLabels || [],
      datasets: [{
        data: data.categoryCounts || [],
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

  private initOrderStatusChart(data: any): void {
    this.orderStatusChartData = {
      labels: data.orderStatusLabels || [],
      datasets: [{
        data: data.orderStatusCounts || [],
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
