import { Component, signal, computed, OnInit, inject } from '@angular/core';
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
import { Api } from '../../api/api';
import { apiOrdersGet, apiOrdersPost } from '../../api/functions';
import { SearchableSelectComponent } from '../../shared/components/searchable-select/searchable-select.component';

interface OrderItem {
  id: string; code: string; type: number; status: number;
  orderDate: string; providerId?: string; providerName?: string;
  customerId?: string; customerName?: string;
  totalAmount: number; discount: number; finalAmount: number;
  note?: string; createdBy?: string; createdAt: string;
}

@Component({
  selector: 'app-orders',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, SelectModule, TagModule, TextareaModule, DatePickerModule, TabsModule, TooltipModule, FloatLabelModule, SearchableSelectComponent],
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

  private api = inject(Api);

  filteredItems = computed(() => {
    let list = this.items() || [];
    const tab = this.activeTab();
    if (tab === 1) list = list.filter(i => i.type === 1);
    else if (tab === 2) list = list.filter(i => i.type === 2);
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => (i.code || '').toLowerCase().includes(s) || (i.providerName || '').toLowerCase().includes(s) || (i.customerName || '').toLowerCase().includes(s));
    if (this.filterStatus() !== null) list = list.filter(i => i.status === this.filterStatus());
    return list;
  });
  totalOrders = computed(() => (this.items() || []).length);
  importCount = computed(() => (this.items() || []).filter(i => i.type === 1).length);
  exportCount = computed(() => (this.items() || []).filter(i => i.type === 2).length);
  totalRevenue = computed(() => (this.items() || []).filter(i => i.type === 2 && i.status === 3).reduce((sum, i) => sum + (i.finalAmount || 0), 0));

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) {}

  ngOnInit() {
    this.loadData();
  }

  async loadData() {
    try {
      const res: any = await this.api.invoke(apiOrdersGet);
      this.items.set(res?.data || []);
    } catch (err) {
      console.error(err);
    }
  }

  formatPrice(val: number): string { return new Intl.NumberFormat('vi-VN').format(val) + 'đ'; }
  getTypeLabel(t: number) { return t === 1 ? 'Nhập kho' : 'Xuất kho'; }
  getTypeSeverity(t: number): 'info' | 'warn' { return t === 1 ? 'info' : 'warn'; }
  getStatusLabel(s: number) { return ['Nháp', 'Chờ xử lý', 'Đã xác nhận', 'Hoàn thành', 'Đã hủy'][s]; }
  getStatusSeverity(s: number): 'secondary' | 'warn' | 'info' | 'success' | 'danger' { return (['secondary', 'warn', 'info', 'success', 'danger'] as const)[s]; }
  setStatusFilter(v: number | null) { this.filterStatus.set(this.filterStatus() === v ? null : v); }

  openNew() { this.form = { type: 1, status: 0, orderDate: new Date(), discount: 0, totalAmount: 0, code: '', note: '' }; this.dialogVisible.set(true); }

  async saveItem() {
    if (!this.form.code) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng nhập mã đơn hàng.', life: 3000 }); return; }
    try {
      await this.api.invoke(apiOrdersPost, { body: this.form });
      this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Đơn hàng mới đã được tạo.', life: 3000 });
      this.dialogVisible.set(false);
      this.loadData();
    } catch (err) {
      console.error(err);
    }
  }
}
