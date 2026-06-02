import { Component, signal, computed, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ConfirmationService, MessageService } from 'primeng/api';

interface ProviderItem {
  id: string; code: string; name: string; phone?: string; email?: string;
  address?: string; taxCode?: string; contactPerson?: string; note?: string;
  isActive: boolean; createdAt: string;
}

@Component({
  selector: 'app-providers',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, TextareaModule, TagModule, TooltipModule, FloatLabelModule],
  templateUrl: './providers.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class Providers implements OnInit {
  items = signal<ProviderItem[]>([]);
  dialogVisible = signal(false);
  isEdit = signal(false);
  searchValue = signal('');
  filterActive = signal<boolean | null>(null);
  form: any = {};

  filteredItems = computed(() => {
    let list = this.items();
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => i.code.toLowerCase().includes(s) || i.name.toLowerCase().includes(s) || i.phone?.toLowerCase().includes(s) || i.email?.toLowerCase().includes(s));
    if (this.filterActive() !== null) list = list.filter(i => i.isActive === this.filterActive());
    return list;
  });
  totalCount = computed(() => this.items().length);
  activeCount = computed(() => this.items().filter(i => i.isActive).length);

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) {}

  ngOnInit() {
    this.items.set([
      { id: '1', code: 'NCC001', name: 'Apple Việt Nam', phone: '028 1234 5678', email: 'contact@apple.vn', address: 'TP.HCM', taxCode: '0123456789', contactPerson: 'Nguyễn Văn A', isActive: true, createdAt: '2026-01-15' },
      { id: '2', code: 'NCC002', name: 'Samsung Electronics', phone: '024 9876 5432', email: 'info@samsung.vn', address: 'Hà Nội', taxCode: '9876543210', contactPerson: 'Trần Thị B', isActive: true, createdAt: '2026-02-20' },
      { id: '3', code: 'NCC003', name: 'Dell Technologies', phone: '028 5555 1234', email: 'sales@dell.vn', address: 'Đà Nẵng', taxCode: '5555123456', contactPerson: 'Lê Văn C', isActive: false, createdAt: '2026-03-10' },
      { id: '4', code: 'NCC004', name: 'Sony Vietnam', phone: '028 7777 8888', email: 'support@sony.vn', address: 'TP.HCM', taxCode: '7777888899', contactPerson: 'Phạm Thị D', isActive: true, createdAt: '2026-04-05' },
    ]);
  }

  setFilter(val: boolean | null) { this.filterActive.set(this.filterActive() === val ? null : val); }
  openNew() { this.form = { isActive: true, code: '', name: '', phone: '', email: '', taxCode: '', contactPerson: '', address: '', note: '' }; this.isEdit.set(false); this.dialogVisible.set(true); }
  editItem(item: ProviderItem) { this.form = { ...item }; this.isEdit.set(true); this.dialogVisible.set(true); }

  deleteItem(item: ProviderItem) {
    this.confirmationService.confirm({
      message: `Bạn có chắc muốn xóa <b>${item.name}</b>?`, header: 'Xác nhận xóa', icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Xóa', rejectLabel: 'Hủy', acceptButtonStyleClass: 'p-button-danger',
      accept: () => { this.items.update(l => l.filter(i => i.id !== item.id)); this.messageService.add({ severity: 'success', summary: 'Đã xóa', detail: `${item.name} đã được xóa.`, life: 3000 }); }
    });
  }

  saveItem() {
    if (!this.form.code || !this.form.name) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng nhập mã và tên.', life: 3000 }); return; }
    if (this.isEdit()) {
      this.items.update(l => l.map(i => i.id === this.form.id ? { ...i, ...this.form } : i));
      this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Nhà cung cấp đã được cập nhật.', life: 3000 });
    } else {
      this.items.update(l => [{ ...this.form, id: crypto.randomUUID(), createdAt: new Date().toISOString().slice(0, 10), isActive: true }, ...l]);
      this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Nhà cung cấp mới đã được tạo.', life: 3000 });
    }
    this.dialogVisible.set(false);
  }
}
