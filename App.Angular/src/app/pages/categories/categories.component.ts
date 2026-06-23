import { Component, signal, computed, OnInit, inject } from '@angular/core';
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
import { Api } from '../../api/api';
import { apiCategoriesGet, apiCategoriesPost, apiCategoriesIdPut, apiCategoriesIdDelete } from '../../api/functions';
import { SearchableSelectComponent } from '../../shared/components/searchable-select/searchable-select.component';

interface CategoryItem {
  id: string; code: string; name: string; description?: string; icon?: string;
  parentId?: string; sortOrder: number; isActive: boolean; createdAt: string;
}

@Component({
  selector: 'app-categories',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, SelectModule, TagModule, TooltipModule, FloatLabelModule, SearchableSelectComponent],
  templateUrl: './categories.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class Categories implements OnInit {
  items = signal<CategoryItem[]>([]);
  dialogVisible = signal(false);
  isEdit = signal(false);
  searchValue = signal('');
  form: any = {};
  autoCode = true;
  parentOptions: { label: string; value: string | null }[] = [];

  private api = inject(Api);

  filteredItems = computed(() => {
    let list = this.items() || [];
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => (i.code || '').toLowerCase().includes(s) || (i.name || '').toLowerCase().includes(s));
    return list;
  });
  totalCount = computed(() => (this.items() || []).length);

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) { }

  ngOnInit() {
    this.loadData();
  }

  async loadData() {
    try {
      const res: any = await this.api.invoke(apiCategoriesGet);
      this.items.set(res?.data || []);
    } catch (err) {
      console.error(err);
    }
  }

  updateParentOptions() {
    const excludeId = this.isEdit() ? this.form.id : null;
    this.parentOptions = [{ label: '— Không có —', value: null }, ...(this.items() || []).filter(i => i.id !== excludeId).map(i => ({ label: i.name, value: i.id }))];
  }

  openNew() { this.form = { isActive: true, sortOrder: 0, code: '', name: '', icon: '', description: '' }; this.autoCode = true; this.isEdit.set(false); this.updateParentOptions(); this.dialogVisible.set(true); }
  editItem(item: CategoryItem) { this.form = { ...item }; this.isEdit.set(true); this.updateParentOptions(); this.dialogVisible.set(true); }

  deleteItem(item: CategoryItem) {
    this.confirmationService.confirm({
      message: `Bạn có chắc muốn xóa <b>${item.name}</b>?`, header: 'Xác nhận xóa', icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Xóa', rejectLabel: 'Hủy', acceptButtonStyleClass: 'p-button-danger',
      accept: async () => {
        try {
          await this.api.invoke(apiCategoriesIdDelete, { id: item.id });
          this.items.update(l => l.filter(i => i.id !== item.id));
          this.messageService.add({ severity: 'success', summary: 'Đã xóa', detail: `${item.name} đã được xóa.`, life: 3000 });
        } catch (err) {
          console.error(err);
        }
      }
    });
  }

  async saveItem() {
    if (!this.form.name) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng nhập tên danh mục.', life: 3000 }); return; }
    const body = { ...this.form, code: this.autoCode ? '' : (this.form.code || '') };
    try {
      if (this.isEdit()) {
        await this.api.invoke(apiCategoriesIdPut, { id: this.form.id, body });
        this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Danh mục đã được cập nhật.', life: 3000 });
      } else {
        await this.api.invoke(apiCategoriesPost, { body });
        this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Danh mục mới đã được tạo.', life: 3000 });
      }
      this.dialogVisible.set(false);
      this.loadData();
    } catch (err) {
      console.error(err);
    }
  }
}
