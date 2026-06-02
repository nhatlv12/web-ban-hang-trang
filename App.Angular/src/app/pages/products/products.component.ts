import { Component, signal, computed, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { TextareaModule } from 'primeng/textarea';
import { TooltipModule } from 'primeng/tooltip';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ConfirmationService, MessageService } from 'primeng/api';

interface ProductItem {
  id: string; code: string; name: string; description?: string;
  categoryId: string; categoryName: string; providerId: string; providerName: string;
  costPrice: number; sellingPrice: number; originalPrice?: number;
  unit: string; image?: string; isNew: boolean; isSale: boolean; isActive: boolean;
  stockQuantity: number; createdAt: string;
}

@Component({
  selector: 'app-products',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, SelectModule, TagModule, ToggleSwitchModule, TextareaModule, TooltipModule, FloatLabelModule],
  templateUrl: './products.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class Products implements OnInit {
  items = signal<ProductItem[]>([]);
  dialogVisible = signal(false);
  isEdit = signal(false);
  searchValue = signal('');
  filterCategory = signal<string | null>(null);
  form: any = {};

  categoryOptions = [{ label: 'Điện thoại', value: '1' }, { label: 'Laptop', value: '2' }, { label: 'Tablet', value: '3' }, { label: 'Phụ kiện', value: '4' }];
  providerOptions = [{ label: 'Apple Việt Nam', value: '1' }, { label: 'Samsung Electronics', value: '2' }, { label: 'Dell Technologies', value: '3' }, { label: 'Sony Vietnam', value: '4' }];

  filteredItems = computed(() => {
    let list = this.items();
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => i.code.toLowerCase().includes(s) || i.name.toLowerCase().includes(s) || i.categoryName.toLowerCase().includes(s));
    if (this.filterCategory()) list = list.filter(i => i.categoryId === this.filterCategory());
    return list;
  });
  totalCount = computed(() => this.items().length);
  totalValue = computed(() => this.items().reduce((sum, i) => sum + i.sellingPrice * i.stockQuantity, 0));
  lowStockCount = computed(() => this.items().filter(i => i.stockQuantity <= 5).length);

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) {}

  ngOnInit() {
    this.items.set([
      { id: '1', code: 'SP001', name: 'iPhone 16 Pro Max 256GB', categoryId: '1', categoryName: 'Điện thoại', providerId: '1', providerName: 'Apple Việt Nam', costPrice: 30000000, sellingPrice: 34990000, originalPrice: 38990000, unit: 'Cái', isNew: true, isSale: true, isActive: true, stockQuantity: 50, createdAt: '2026-05-01' },
      { id: '2', code: 'SP002', name: 'MacBook Pro M4 14"', categoryId: '2', categoryName: 'Laptop', providerId: '1', providerName: 'Apple Việt Nam', costPrice: 43000000, sellingPrice: 49990000, unit: 'Cái', isNew: true, isSale: false, isActive: true, stockQuantity: 20, createdAt: '2026-05-10' },
      { id: '3', code: 'SP003', name: 'AirPods Pro 3', categoryId: '4', categoryName: 'Phụ kiện', providerId: '1', providerName: 'Apple Việt Nam', costPrice: 5500000, sellingPrice: 6990000, originalPrice: 7990000, unit: 'Cái', isNew: false, isSale: true, isActive: true, stockQuantity: 100, createdAt: '2026-04-15' },
      { id: '4', code: 'SP004', name: 'Galaxy S25 Ultra', categoryId: '1', categoryName: 'Điện thoại', providerId: '2', providerName: 'Samsung Electronics', costPrice: 26000000, sellingPrice: 31990000, unit: 'Cái', isNew: true, isSale: false, isActive: true, stockQuantity: 35, createdAt: '2026-05-20' },
      { id: '5', code: 'SP005', name: 'iPad Pro M4 11"', categoryId: '3', categoryName: 'Tablet', providerId: '1', providerName: 'Apple Việt Nam', costPrice: 23000000, sellingPrice: 27990000, unit: 'Cái', isNew: false, isSale: false, isActive: true, stockQuantity: 3, createdAt: '2026-03-01' },
    ]);
  }

  formatPrice(val: number): string { return new Intl.NumberFormat('vi-VN').format(val) + 'đ'; }
  setCategoryFilter(val: string | null) { this.filterCategory.set(this.filterCategory() === val ? null : val); }
  openNew() { this.form = { isActive: true, isNew: false, isSale: false, costPrice: 0, sellingPrice: 0, unit: 'Cái', code: '', name: '', description: '' }; this.isEdit.set(false); this.dialogVisible.set(true); }
  editItem(item: ProductItem) { this.form = { ...item }; this.isEdit.set(true); this.dialogVisible.set(true); }

  deleteItem(item: ProductItem) {
    this.confirmationService.confirm({
      message: `Bạn có chắc muốn xóa <b>${item.name}</b>?`, header: 'Xác nhận xóa', icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Xóa', rejectLabel: 'Hủy', acceptButtonStyleClass: 'p-button-danger',
      accept: () => { this.items.update(l => l.filter(i => i.id !== item.id)); this.messageService.add({ severity: 'success', summary: 'Đã xóa', detail: `${item.name} đã được xóa.`, life: 3000 }); }
    });
  }

  saveItem() {
    if (!this.form.code || !this.form.name) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng nhập mã và tên sản phẩm.', life: 3000 }); return; }
    if (this.isEdit()) {
      this.items.update(l => l.map(i => i.id === this.form.id ? { ...i, ...this.form } : i));
      this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Sản phẩm đã được cập nhật.', life: 3000 });
    } else {
      this.items.update(l => [{ ...this.form, id: crypto.randomUUID(), createdAt: new Date().toISOString().slice(0, 10), isActive: true, stockQuantity: 0 }, ...l]);
      this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Sản phẩm mới đã được tạo.', life: 3000 });
    }
    this.dialogVisible.set(false);
  }
}
