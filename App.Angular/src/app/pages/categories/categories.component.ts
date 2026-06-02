import { Component, signal, computed, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ConfirmationService, MessageService } from 'primeng/api';

interface CategoryItem {
  id: string; code: string; name: string; description?: string; icon?: string;
  parentId?: string; sortOrder: number; isActive: boolean; createdAt: string;
}

@Component({
  selector: 'app-categories',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, SelectModule, TagModule, TooltipModule, FloatLabelModule],
  templateUrl: './categories.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class Categories implements OnInit {
  items = signal<CategoryItem[]>([]);
  dialogVisible = signal(false);
  isEdit = signal(false);
  searchValue = signal('');
  form: any = {};
  parentOptions: { label: string; value: string | null }[] = [];

  filteredItems = computed(() => {
    let list = this.items();
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => i.code.toLowerCase().includes(s) || i.name.toLowerCase().includes(s));
    return list;
  });
  totalCount = computed(() => this.items().length);

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) { }

  ngOnInit() {
    this.items.set([
      { id: '1', code: 'DM001', name: 'Điện thoại', icon: 'pi pi-mobile', sortOrder: 1, isActive: true, createdAt: '2026-01-01' },
      { id: '2', code: 'DM002', name: 'Laptop', icon: 'pi pi-desktop', sortOrder: 2, isActive: true, createdAt: '2026-01-01' },
      { id: '3', code: 'DM003', name: 'Tablet', icon: 'pi pi-tablet', sortOrder: 3, isActive: true, createdAt: '2026-01-01' },
      { id: '4', code: 'DM004', name: 'Phụ kiện', icon: 'pi pi-headphones', sortOrder: 4, isActive: true, createdAt: '2026-01-01' },
      { id: '5', code: 'DM005', name: 'Đồng hồ', icon: 'pi pi-clock', sortOrder: 5, isActive: false, createdAt: '2026-01-01' },
    ]);
  }

  updateParentOptions() {
    const excludeId = this.isEdit() ? this.form.id : null;
    this.parentOptions = [{ label: '— Không có —', value: null }, ...this.items().filter(i => i.id !== excludeId).map(i => ({ label: i.name, value: i.id }))];
  }

  openNew() { this.form = { isActive: true, sortOrder: 0, code: '', name: '', icon: '', description: '' }; this.isEdit.set(false); this.updateParentOptions(); this.dialogVisible.set(true); }
  editItem(item: CategoryItem) { this.form = { ...item }; this.isEdit.set(true); this.updateParentOptions(); this.dialogVisible.set(true); }

  deleteItem(item: CategoryItem) {
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
      this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Danh mục đã được cập nhật.', life: 3000 });
    } else {
      this.items.update(l => [{ ...this.form, id: crypto.randomUUID(), createdAt: new Date().toISOString().slice(0, 10), isActive: true }, ...l]);
      this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Danh mục mới đã được tạo.', life: 3000 });
    }
    this.dialogVisible.set(false);
  }
}
