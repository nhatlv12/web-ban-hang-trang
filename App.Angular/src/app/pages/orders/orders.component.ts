import { Component, signal, computed, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { TextareaModule } from 'primeng/textarea';
import { DatePickerModule } from 'primeng/datepicker';
import { TabsModule } from 'primeng/tabs';
import { TooltipModule } from 'primeng/tooltip';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ConfirmationService, MessageService } from 'primeng/api';

interface OrderItem {
  id: string; code: string; type: number; status: number;
  orderDate: string; providerId?: string; providerName?: string;
  customerId?: string; customerName?: string;
  totalAmount: number; discount: number; finalAmount: number;
  note?: string; createdBy?: string; createdAt: string;
}

@Component({
  selector: 'app-orders',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, SelectModule, TagModule, TextareaModule, DatePickerModule, TabsModule, TooltipModule, FloatLabelModule],
  templateUrl: './orders.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class Orders implements OnInit {
  items = signal<OrderItem[]>([]);
  dialogVisible = signal(false);
  searchValue = signal('');
  activeTab = signal(0);
  filterStatus = signal<number | null>(null);
  form: any = {};

  typeOptions = [{ label: 'Phiếu nhập', value: 1 }, { label: 'Phiếu xuất', value: 2 }];
  statusOptions = [{ label: 'Nháp', value: 0 }, { label: 'Chờ xử lý', value: 1 }, { label: 'Đã xác nhận', value: 2 }, { label: 'Hoàn thành', value: 3 }, { label: 'Đã hủy', value: 4 }];
  providerOptions = [{ label: 'Apple Việt Nam', value: '1' }, { label: 'Samsung Electronics', value: '2' }];
  customerOptions = [{ label: 'Nguyễn Văn An', value: '1' }, { label: 'Trần Thị Bình', value: '2' }, { label: 'Lê Hoàng Cường', value: '3' }];

  filteredItems = computed(() => {
    let list = this.items();
    const tab = this.activeTab();
    if (tab === 1) list = list.filter(i => i.type === 1);
    else if (tab === 2) list = list.filter(i => i.type === 2);
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => i.code.toLowerCase().includes(s) || i.providerName?.toLowerCase().includes(s) || i.customerName?.toLowerCase().includes(s));
    if (this.filterStatus() !== null) list = list.filter(i => i.status === this.filterStatus());
    return list;
  });
  totalOrders = computed(() => this.items().length);
  importCount = computed(() => this.items().filter(i => i.type === 1).length);
  exportCount = computed(() => this.items().filter(i => i.type === 2).length);
  totalRevenue = computed(() => this.items().filter(i => i.type === 2 && i.status === 3).reduce((sum, i) => sum + i.finalAmount, 0));

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) {}

  ngOnInit() {
    this.items.set([
      { id: '1', code: 'PN-2026060001', type: 1, status: 3, orderDate: '2026-06-01', providerName: 'Apple Việt Nam', totalAmount: 900000000, discount: 0, finalAmount: 900000000, createdBy: 'Admin', createdAt: '2026-06-01' },
      { id: '2', code: 'PX-2026060001', type: 2, status: 3, orderDate: '2026-06-01', customerName: 'Nguyễn Văn An', totalAmount: 34990000, discount: 0, finalAmount: 34990000, createdBy: 'Admin', createdAt: '2026-06-01' },
      { id: '3', code: 'PX-2026060002', type: 2, status: 1, orderDate: '2026-06-02', customerName: 'Trần Thị Bình', totalAmount: 56980000, discount: 1000000, finalAmount: 55980000, createdBy: 'Admin', createdAt: '2026-06-02' },
      { id: '4', code: 'PN-2026050001', type: 1, status: 4, orderDate: '2026-05-30', providerName: 'Samsung Electronics', totalAmount: 500000000, discount: 0, finalAmount: 500000000, createdBy: 'Admin', createdAt: '2026-05-30' },
      { id: '5', code: 'PX-2026060003', type: 2, status: 2, orderDate: '2026-06-02', customerName: 'Lê Hoàng Cường', totalAmount: 27990000, discount: 500000, finalAmount: 27490000, createdBy: 'Nhân viên', createdAt: '2026-06-02' },
    ]);
  }

  formatPrice(val: number): string { return new Intl.NumberFormat('vi-VN').format(val) + 'đ'; }
  getTypeLabel(t: number) { return t === 1 ? 'Nhập kho' : 'Xuất kho'; }
  getTypeSeverity(t: number): 'info' | 'warn' { return t === 1 ? 'info' : 'warn'; }
  getStatusLabel(s: number) { return ['Nháp', 'Chờ xử lý', 'Đã xác nhận', 'Hoàn thành', 'Đã hủy'][s]; }
  getStatusSeverity(s: number): 'secondary' | 'warn' | 'info' | 'success' | 'danger' { return (['secondary', 'warn', 'info', 'success', 'danger'] as const)[s]; }
  setStatusFilter(v: number | null) { this.filterStatus.set(this.filterStatus() === v ? null : v); }

  openNew() { this.form = { type: 1, status: 0, orderDate: new Date(), discount: 0, totalAmount: 0, code: '', note: '' }; this.dialogVisible.set(true); }

  saveItem() {
    if (!this.form.code) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng nhập mã đơn hàng.', life: 3000 }); return; }
    const totalAmount = this.form.totalAmount || 0;
    const providerName = this.providerOptions.find(o => o.value === this.form.providerId)?.label;
    const customerName = this.customerOptions.find(o => o.value === this.form.customerId)?.label;
    this.items.update(l => [{ ...this.form, id: crypto.randomUUID(), totalAmount, finalAmount: totalAmount - (this.form.discount || 0), providerName, customerName, createdAt: new Date().toISOString().slice(0, 10), createdBy: 'Admin' }, ...l]);
    this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Đơn hàng mới đã được tạo.', life: 3000 });
    this.dialogVisible.set(false);
  }
}
