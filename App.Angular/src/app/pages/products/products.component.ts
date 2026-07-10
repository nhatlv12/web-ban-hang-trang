import { Component, signal, computed, OnInit, inject } from '@angular/core';
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
import { HttpClient } from '@angular/common/http';
import { Api } from '../../api/api';
import { apiProductsGet, apiProductsIdDelete, apiCategoriesGet } from '../../api/functions';
import { SearchableSelectComponent } from '../../shared/components/searchable-select/searchable-select.component';
import { environment } from '../../../environments/environment';

interface ProductItem {
  id: string; code: string; name: string; description?: string;
  categoryId: string; categoryName: string;
  unit: string; image?: string; isActive: boolean;
  sellingPrice: number; stockQuantity: number; createdAt: string;
}

@Component({
  selector: 'app-products',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, SelectModule, TagModule, ToggleSwitchModule, TextareaModule, TooltipModule, FloatLabelModule, SearchableSelectComponent],
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

  categoryOptions = signal<{ label: string; value: string }[]>([]);

  private api = inject(Api);

  filteredItems = computed(() => {
    let list = this.items() || [];
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => (i.code || '').toLowerCase().includes(s) || (i.name || '').toLowerCase().includes(s) || (i.categoryName || '').toLowerCase().includes(s));
    if (this.filterCategory()) list = list.filter(i => i.categoryId === this.filterCategory());
    return list;
  });
  totalCount = computed(() => (this.items() || []).length);
  lowStockCount = computed(() => (this.items() || []).filter(i => (i.stockQuantity || 0) <= 5).length);

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) {}

  ngOnInit() {
    this.loadCategories();
    this.loadData();
  }

  async loadCategories() {
    try {
      const res: any = await this.api.invoke(apiCategoriesGet);
      const categories = res?.data || [];
      this.categoryOptions.set(categories.map((c: any) => ({ label: c.name, value: c.id })));
    } catch (err) {
      console.error('Failed to load categories', err);
    }
  }

  async loadData() {
    try {
      const res: any = await this.api.invoke(apiProductsGet);
      this.items.set(res?.data || []);
    } catch (err) {
      console.error(err);
    }
  }

  formatPrice(val: number): string { return new Intl.NumberFormat('vi-VN').format(val) + 'đ'; }
  setCategoryFilter(val: string | null) { this.filterCategory.set(this.filterCategory() === val ? null : val); }

  imagePreview = signal<string | null>(null);
  isDragging = false;
  autoCode = true;
  private selectedFile: File | null = null;
  private http = inject(HttpClient);

  openNew() {
    this.form = { isActive: true, unit: 'Cái', code: '', name: '', description: '', sellingPrice: 0 };
    this.autoCode = true;
    this.imagePreview.set(null);
    this.selectedFile = null;
    this.isEdit.set(false);
    this.dialogVisible.set(true);
  }

  editItem(item: ProductItem) {
    this.form = { ...item };
    this.imagePreview.set(item.image ? this.getFullImageUrl(item.image) : null);
    this.selectedFile = null;
    this.autoCode = false;
    this.isEdit.set(true);
    this.dialogVisible.set(true);
  }

  getFullImageUrl(path: string): string {
    if (path.startsWith('http')) return path;
    const base = environment.apiUrl.replace('/api', '');
    return base + path;
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.processFile(files[0]);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.processFile(input.files[0]);
      input.value = '';
    }
  }

  private processFile(file: File) {
    if (!file.type.startsWith('image/')) {
      this.messageService.add({ severity: 'warn', summary: 'Sai định dạng', detail: 'Vui lòng chọn file ảnh (PNG, JPG, WEBP).', life: 3000 });
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      this.messageService.add({ severity: 'warn', summary: 'File quá lớn', detail: 'Kích thước ảnh không được vượt quá 5MB.', life: 3000 });
      return;
    }
    this.selectedFile = file;
    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview.set(reader.result as string);
    };
    reader.readAsDataURL(file);
  }

  removeImage(event: Event) {
    event.stopPropagation();
    this.imagePreview.set(null);
    this.selectedFile = null;
    this.form.image = null;
  }

  deleteItem(item: ProductItem) {
    this.confirmationService.confirm({
      message: `Bạn có chắc muốn xóa <b>${item.name}</b>?`, header: 'Xác nhận xóa', icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Xóa', rejectLabel: 'Hủy', acceptButtonStyleClass: 'p-button-danger',
      accept: async () => { 
        try {
          await this.api.invoke(apiProductsIdDelete, { id: item.id });
          this.items.update(l => l.filter(i => i.id !== item.id)); 
          this.messageService.add({ severity: 'success', summary: 'Đã xóa', detail: `${item.name} đã được xóa.`, life: 3000 }); 
        } catch(err) {
          console.error(err);
        }
      }
    });
  }

  private buildFormData(): FormData {
    const fd = new FormData();
    fd.append('Code', this.isEdit() ? (this.form.code || '') : (this.autoCode ? '' : (this.form.code || '')));
    fd.append('Name', this.form.name || '');
    fd.append('Description', this.form.description || '');
    fd.append('CategoryId', this.form.categoryId || '');
    fd.append('Unit', this.form.unit || '');
    fd.append('SellingPrice', String(this.form.sellingPrice || 0));
    fd.append('CostPrice', String(this.form.costPrice || 0));
    if (this.isEdit()) {
      fd.append('Id', this.form.id || '');
      fd.append('IsActive', String(this.form.isActive ?? true));
    }
    if (this.selectedFile) {
      fd.append('Image', this.selectedFile, this.selectedFile.name);
    }
    return fd;
  }

  async saveItem() {
    if ((!this.autoCode && !this.form.code) || !this.form.name || !this.form.categoryId) {
      this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng nhập các trường bắt buộc.', life: 3000 });
      return;
    }
    try {
      const fd = this.buildFormData();
      const baseUrl = environment.apiUrl.replace('/api', '');
      if (this.isEdit()) {
        await this.http.put(`${baseUrl}/api/products/${this.form.id}`, fd).toPromise();
        this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Sản phẩm đã được cập nhật.', life: 3000 });
      } else {
        await this.http.post(`${baseUrl}/api/products`, fd).toPromise();
        this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Sản phẩm mới đã được tạo.', life: 3000 });
      }
      this.dialogVisible.set(false);
      this.loadData();
    } catch (err: any) {
      const msg = err?.error?.message || 'Có lỗi xảy ra.';
      this.messageService.add({ severity: 'error', summary: 'Lỗi', detail: msg, life: 4000 });
    }
  }
}
