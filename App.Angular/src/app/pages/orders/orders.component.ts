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
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Api } from '../../api/api';
import { apiOrdersGet, apiOrdersPost, apiCustomersGet, apiProductsGet, apiOrdersIdGet, apiOrdersIdStatusPut, apiOrdersIdPut } from '../../api/functions';
import { OrderType } from '../../api/models/order-type';
import { SearchableSelectComponent } from '../../shared/components/searchable-select/searchable-select.component';

interface OrderItem {
  id: string; code: string; type: number; status: number;
  orderDate: string; customerId?: string; customerName?: string;
  totalAmount: number; discount: number; shippingFee: number; finalAmount: number;
  note?: string; createdBy?: string; createdAt: string;
}

interface DetailLine {
  productId: string; productName: string;
  quantity: number; unitPrice: number; discount: number; tax: number;
  costPrice?: number; discountType?: 'amount' | 'percent';
}

@Component({
  selector: 'app-orders',
  imports: [CommonModule, DatePipe, FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, SelectModule, TagModule, TextareaModule, DatePickerModule, TooltipModule, FloatLabelModule, InputGroupModule, InputGroupAddonModule, SearchableSelectComponent],
  templateUrl: './orders.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class Orders implements OnInit {
  items = signal<OrderItem[]>([]);
  dialogVisible = signal(false);
  isSaving = signal(false);
  viewDialogVisible = signal(false);
  viewedOrder = signal<any>(null);
  searchValue = signal('');
  filterStatus = signal<number | null>(null);
  filterCustomer = signal<string | null>(null);
  filterFromDate = signal<Date | null>(null);
  filterToDate = signal<Date | null>(null);
  filterProduct = signal<string | null>(null);
  form: any = {};
  details = signal<DetailLine[]>([]);

  customerOptions: any[] = [];
  productOptions: any[] = [];
  filterProductOptions: any[] = [];
  statusOptions = [{label:'Trạng thái',value:null}, {label:'Chờ xử lý',value:1}, {label:'Hoàn thành',value:3}, {label:'Đã hủy',value:4}];

  private api = inject(Api);

  filteredItems = computed(() => {
    let list = this.items() || [];
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => (i.code || '').toLowerCase().includes(s) || (i.customerName || '').toLowerCase().includes(s));
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

    const custId = this.filterCustomer();
    if (custId) list = list.filter(i => i.customerId === custId);

    return list;
  });
  totalOrders = computed(() => (this.items() || []).length);
  totalRevenue = computed(() => (this.items() || []).filter(i => i.status === 3).reduce((sum, i) => sum + (i.finalAmount || 0), 0));
  totalValue = computed(() => (this.items() || []).reduce((sum, item) => sum + item.finalAmount, 0));

  getTotalDiscount(item: any): number {
    const lineDiscount = item.details ? item.details.reduce((sum: number, d: any) => sum + (d.discount || 0), 0) : 0;
    return (item.discount || 0) + lineDiscount;
  }

  getTaxPercent(line: any): number {
    if (!line || !line.tax) return 0;
    const base = (line.quantity * line.unitPrice) - (line.discount || 0);
    if (base <= 0) return 0;
    return Math.round((line.tax / base) * 100);
  }

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) { }

  ngOnInit() {
    this.loadData();
    this.loadCustomers();
    this.loadProducts();
  }

  async loadData() {
    try {
      const res: any = await this.api.invoke(apiOrdersGet, { type: 2 });
      this.items.set(res?.data || []);
    } catch (err) { console.error(err); }
  }

  async loadCustomers() {
    try {
      const res: any = await this.api.invoke(apiCustomersGet);
      this.customerOptions = (res?.data || []).map((c: any) => ({ label: c.fullName, value: c.id }));
    } catch (err) { console.error(err); }
  }

  async loadProducts() {
    try {
      const res: any = await this.api.invoke(apiProductsGet);
      const allProducts = res?.data || [];
      
      this.filterProductOptions = allProducts.map((p: any) => ({ 
        label: `${p.code} - ${p.name}`, 
        value: p.id 
      }));

      this.productOptions = allProducts
        .map((p: any) => ({ 
          label: `${p.code} - ${p.name} (Tồn: ${p.stockQuantity})`, 
          value: p.id, 
          unit: p.unit, 
          sellingPrice: p.sellingPrice,
          costPrice: p.costPrice
        }));
    } catch (err) { console.error(err); }
  }

  formatPrice(val: number): string { return new Intl.NumberFormat('vi-VN').format(val) + 'đ'; }
  getStatusLabel(s: number) { return ['Nháp', 'Chờ xử lý', 'Đã xác nhận', 'Hoàn thành', 'Đã hủy'][s]; }
  getStatusSeverity(s: number): 'secondary' | 'warn' | 'info' | 'success' | 'danger' { return (['secondary', 'warn', 'info', 'success', 'danger'] as const)[s]; }
  setStatusFilter(v: number | null) { this.filterStatus.set(this.filterStatus() === v ? null : v); }

  openNew() {
    this.form = { type: 2, status: 0, orderDate: new Date(), discountValue: null, discountType: 'amount', shippingFee: null, code: '', note: '', customerId: null };
    this.details.set([{ productId: '', productName: '', quantity: 1, unitPrice: null as any, discount: null as any, tax: null as any, costPrice: null as any, discountType: 'amount' }]);
    this.dialogVisible.set(true);
  }

  addLine() { this.details.update(d => [...d, { productId: '', productName: '', quantity: 1, unitPrice: null as any, discount: null as any, tax: null as any, costPrice: null as any, discountType: 'amount' }]); }
  removeLine(i: number) { this.details.update(d => d.filter((_, idx) => idx !== i)); }

  async editItem(id: string) {
    try {
      const res: any = await this.api.invoke(apiOrdersIdGet, { id });
      if (res?.data) {
        const order = res.data;
        if (order.status > 1) {
          this.messageService.add({ severity: 'warn', summary: 'Cảnh báo', detail: 'Chỉ có thể sửa đơn ở trạng thái Nháp hoặc Chờ xử lý.', life: 3000 });
          return;
        }
        this.form = {
          id: order.id,
          code: order.code,
          type: order.type,
          status: order.status,
          orderDate: new Date(order.orderDate),
          customerId: order.customerId,
          discountValue: order.discount,
          discountType: 'amount',
          shippingFee: order.shippingFee,
          note: order.note || ''
        };
        this.details.set(order.details.map((d: any) => {
          const rawBase = d.quantity * d.unitPrice;
          const base = rawBase - (d.discount || 0);
          const taxPercent = base > 0 ? Math.round(((d.tax || 0) / base) * 100) : 0;
          return {
            productId: d.productId,
            productName: d.productName,
            quantity: d.quantity,
            unitPrice: d.unitPrice,
            discount: d.discount,
            tax: taxPercent,
            discountType: 'amount',
            costPrice: 0
          };
        }));
        this.dialogVisible.set(true);
      }
    } catch (err) {
      console.error(err);
      this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể tải chi tiết đơn hàng để sửa.', life: 3000 });
    }
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
      this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể tải chi tiết đơn hàng.', life: 3000 });
    }
  }

  async changeStatus(newStatus: number) {
    if (!this.viewedOrder()) return;
    const orderId = this.viewedOrder().id;
    try {
      await this.api.invoke(apiOrdersIdStatusPut, { id: orderId, body: { id: orderId, status: newStatus as any } });
      this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Cập nhật trạng thái thành công.', life: 3000 });
      this.viewedOrder.update((o: any) => ({ ...o, status: newStatus }));
      this.loadData();
      this.loadProducts();
    } catch (err: any) {
      console.error(err);
      const msg = err.error?.message || 'Không thể cập nhật trạng thái.';
      this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: msg, life: 3000 });
    }
  }

  onProductSelect(index: number, productId: string) {
    const product = this.productOptions.find(p => p.value === productId);
    if (product) {
      this.details.update(d => {
        const updated = [...d];
        updated[index] = { ...updated[index], productId, productName: product.label, unitPrice: product.sellingPrice || 0, costPrice: product.costPrice || 0 };
        return updated;
      });
    }
  }

  calcLineTotal(line: DetailLine): number {
    const rawBase = line.quantity * line.unitPrice;
    const discountAmt = line.discountType === 'percent' ? rawBase * (line.discount || 0) / 100 : (line.discount || 0);
    const base = rawBase - discountAmt;
    const taxAmount = base * (line.tax || 0) / 100;
    return base + taxAmount;
  }

  calcSubtotal(): number { return this.details().reduce((sum, l) => sum + this.calcLineTotal(l), 0); }

  calcProductDiscountTotal(): number {
    return this.details().reduce((sum, line) => {
      const rawBase = (line.quantity || 0) * (line.unitPrice || 0);
      const discountAmt = line.discountType === 'percent' ? rawBase * (line.discount || 0) / 100 : (line.discount || 0);
      return sum + discountAmt;
    }, 0);
  }

  calcDiscountAmount(): number {
    if (this.form.discountType === 'percent') return this.calcSubtotal() * (this.form.discountValue || 0) / 100;
    return this.form.discountValue || 0;
  }

  toggleDiscountType() { this.form.discountType = this.form.discountType === 'percent' ? 'amount' : 'percent'; }

  calcTotal(): number { return this.calcSubtotal() - this.calcDiscountAmount() + (this.form.shippingFee || 0); }

  async saveItem() {
    if (!this.form.customerId) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng chọn khách hàng.', life: 3000 }); return; }
    const validDetails = this.details().filter(d => d.productId && d.quantity > 0);
    if (validDetails.length === 0) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng thêm ít nhất 1 sản phẩm.', life: 3000 }); return; }

    const body = {
      code: this.form.code || '',
      type: 2 as OrderType,
      orderDate: this.form.orderDate,
      providerId: null,
      customerId: this.form.customerId,
      discount: this.calcDiscountAmount(),
      shippingFee: this.form.shippingFee || 0,
      note: this.form.note || '',
      createdBy: 'Admin',
      details: validDetails.map(d => {
        const rawBase = d.quantity * d.unitPrice;
        const discountAmt = d.discountType === 'percent' ? rawBase * (d.discount || 0) / 100 : (d.discount || 0);
        return {
          productId: d.productId,
          quantity: d.quantity,
          unitPrice: d.unitPrice,
          discount: discountAmt,
          tax: (rawBase - discountAmt) * (d.tax || 0) / 100
        };
      })
    };

    this.isSaving.set(true);
    try {
      if (this.form.id) {
        await this.api.invoke(apiOrdersIdPut, { id: this.form.id, body });
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Cập nhật đơn hàng bán thành công.', life: 3000 });
      } else {
        await this.api.invoke(apiOrdersPost, { body });
        this.messageService.add({ severity: 'success', summary: 'Thành công', detail: 'Đơn hàng bán đã được tạo.', life: 3000 });
      }
      this.dialogVisible.set(false);
      this.loadData();
      this.loadProducts();
    } catch (err) {
      console.error(err);
      this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: 'Không thể lưu đơn hàng.', life: 3000 });
    } finally {
      this.isSaving.set(false);
    }
  }
}
