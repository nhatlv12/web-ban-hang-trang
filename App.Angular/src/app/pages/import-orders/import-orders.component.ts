import { Component, signal, computed, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
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
import { TooltipModule } from 'primeng/tooltip';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Api } from '../../api/api';
import { apiOrdersGet, apiOrdersPost, apiOrdersIdGet, apiOrdersIdStatusPut, apiProvidersGet, apiProductsGet } from '../../api/functions';
import { OrderType } from '../../api/models/order-type';
import { SearchableSelectComponent } from '../../shared/components/searchable-select/searchable-select.component';

interface OrderItem {
  id: string; code: string; type: number; status: number;
  orderDate: string; providerId?: string; providerName?: string;
  totalAmount: number; discount: number; shippingFee: number; finalAmount: number;
  note?: string; createdBy?: string; createdAt: string;
}

interface DetailLine {
  productId: string; productName: string; providerId: string;
  quantity: number; unitPrice: number;
}

@Component({
  selector: 'app-import-orders',
  imports: [CommonModule, DatePipe, FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, SelectModule, TagModule, TextareaModule, DatePickerModule, TooltipModule, FloatLabelModule, SearchableSelectComponent],
  templateUrl: './import-orders.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class ImportOrders implements OnInit {
  items = signal<OrderItem[]>([]);
  dialogVisible = signal(false);
  viewDialogVisible = signal(false);
  viewedOrder = signal<any>(null);
  searchValue = signal('');
  filterStatus = signal<number | null>(null);
  filterFromDate = signal<Date | null>(null);
  filterToDate = signal<Date | null>(null);
  filterProduct = signal<string | null>(null);
  form: any = {};
  details = signal<DetailLine[]>([]);

  providerOptions: any[] = [];
  productOptions: any[] = [];
  statusOptions = [{label:'Trạng thái',value:null}, {label:'Chờ xử lý',value:1}, {label:'Hoàn thành',value:3}, {label:'Đã hủy',value:4}];
  discountTypeOptions = [{label: 'VNĐ', value: 'VND'}, {label: '%', value: '%'}];

  private api = inject(Api);

  filteredItems = computed(() => {
    let list = this.items() || [];
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => (i.code || '').toLowerCase().includes(s) || (i.providerName || '').toLowerCase().includes(s));
    if (this.filterStatus() !== null) list = list.filter(i => i.status === this.filterStatus());
    
    const from = this.filterFromDate();
    if (from) list = list.filter(i => new Date(i.orderDate) >= from);
    
    const to = this.filterToDate();
    if (to) {
      const toEndOfDay = new Date(to);
      toEndOfDay.setHours(23, 59, 59, 999);
      list = list.filter(i => new Date(i.orderDate) <= toEndOfDay);
    }

    const prodId = this.filterProduct();
    if (prodId) list = list.filter(i => (i as any).details?.some((d: any) => d.productId === prodId));
    
    return list;
  });
  totalOrders = computed(() => (this.items() || []).length);
  totalCost = computed(() => (this.items() || []).filter(i => i.status === 3).reduce((sum, i) => sum + (i.finalAmount || 0), 0));

  constructor(private messageService: MessageService) { }

  ngOnInit() {
    this.loadData();
    this.loadProviders();
    this.loadProducts();
  }

  async loadData() {
    try {
      const res: any = await this.api.invoke(apiOrdersGet, { type: 1 });
      this.items.set(res?.data || []);
    } catch (err) { console.error(err); }
  }

  async loadProviders() {
    try {
      const res: any = await this.api.invoke(apiProvidersGet);
      this.providerOptions = (res?.data || []).map((p: any) => ({ label: p.name, value: p.id }));
    } catch (err) { console.error(err); }
  }

  async loadProducts() {
    try {
      const res: any = await this.api.invoke(apiProductsGet);
      this.productOptions = (res?.data || []).map((p: any) => ({ label: `${p.code} - ${p.name}`, value: p.id, unit: p.unit, costPrice: p.costPrice }));
    } catch (err) { console.error(err); }
  }

  formatPrice(val: number): string { return new Intl.NumberFormat('vi-VN').format(val) + 'đ'; }
  getStatusLabel(s: number) { return ['Nháp', 'Chờ xử lý', 'Đã xác nhận', 'Hoàn thành', 'Đã hủy'][s]; }
  getStatusSeverity(s: number): 'secondary' | 'warn' | 'info' | 'success' | 'danger' { return (['secondary', 'warn', 'info', 'success', 'danger'] as const)[s]; }
  setStatusFilter(v: number | null) { this.filterStatus.set(this.filterStatus() === v ? null : v); }

  openNew() {
    this.form = { type: 1, status: 0, orderDate: new Date(), discount: null, discountType: 'VND', shippingFee: null, code: '', note: '' };
    this.details.set([{ productId: '', productName: '', providerId: '', quantity: 1, unitPrice: null as any }]);
    this.dialogVisible.set(true);
  }

  async viewItem(id: string) {
    try {
      const res: any = await this.api.invoke(apiOrdersIdGet, { id });
      if (res?.data) {
        this.viewedOrder.set(res.data);
        this.viewDialogVisible.set(true);
      }
    } catch (err) {
      console.error(err);
      this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể tải chi tiết phiếu nhập.', life: 3000 });
    }
  }

  async changeStatus(newStatus: number) {
    if (!this.viewedOrder()) return;
    const orderId = this.viewedOrder().id;
    try {
      await this.api.invoke(apiOrdersIdStatusPut, { id: orderId, body: { id: orderId, status: newStatus as any } });
      this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Cập nhật trạng thái thành công.', life: 3000 });
      // Update local view
      this.viewedOrder.update((o: any) => ({ ...o, status: newStatus }));
      this.loadData();
      this.loadProducts();
    } catch (err: any) {
      console.error(err);
      const msg = err.error?.message || 'Không thể cập nhật trạng thái.';
      this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: msg, life: 3000 });
    }
  }

  addLine() { this.details.update(d => [...d, { productId: '', productName: '', providerId: '', quantity: 1, unitPrice: null as any }]); }
  removeLine(i: number) { this.details.update(d => d.filter((_, idx) => idx !== i)); }

  onProductSelect(index: number, productId: string) {
    const product = this.productOptions.find(p => p.value === productId);
    if (product) {
      this.details.update(d => {
        const updated = [...d];
        updated[index] = { ...updated[index], productId, productName: product.label, unitPrice: product.costPrice || 0 };
        return updated;
      });
    }
  }

  calcLineTotal(line: DetailLine): number { return line.quantity * line.unitPrice; }

  calcSubtotal(): number { return this.details().reduce((sum, l) => sum + this.calcLineTotal(l), 0); }

  getDiscountAmount(): number {
    const sub = this.calcSubtotal();
    if (this.form.discountType === '%') {
      return (sub * (this.form.discount || 0)) / 100;
    }
    return this.form.discount || 0;
  }

  calcTotal(): number { return this.calcSubtotal() - this.getDiscountAmount() + (this.form.shippingFee || 0); }

  async saveItem() {
    const validDetails = this.details().filter(d => d.productId && d.quantity > 0);
    if (validDetails.length === 0) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng thêm ít nhất 1 sản phẩm.', life: 3000 }); return; }

    const body = {
      code: this.form.code || '',
      type: 1 as OrderType,
      orderDate: this.form.orderDate,
      providerId: null,
      customerId: null,
      discount: this.getDiscountAmount(),
      shippingFee: this.form.shippingFee || 0,
      note: this.form.note || '',
      createdBy: 'Admin',
      details: validDetails.map(d => ({
        productId: d.productId,
        quantity: d.quantity,
        unitPrice: d.unitPrice,
        discount: 0,
        tax: 0,
        providerId: d.providerId || null
      }))
    };

    try {
      await this.api.invoke(apiOrdersPost, { body });
      this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Phiếu nhập hàng đã được tạo.', life: 3000 });
      this.dialogVisible.set(false);
      this.loadData();
      this.loadProducts();
    } catch (err) {
      console.error(err);
    }
  }
}
