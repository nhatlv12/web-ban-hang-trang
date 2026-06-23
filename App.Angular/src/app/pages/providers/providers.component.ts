import { Component, signal, computed, OnInit, inject } from '@angular/core';
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
import { Api } from '../../api/api';
import { apiProvidersGet, apiProvidersPost, apiProvidersIdPut, apiProvidersIdDelete } from '../../api/functions';

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
  autoCode = true;

  private api = inject(Api);

  filteredItems = computed(() => {
    let list = this.items() || [];
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => (i.code || '').toLowerCase().includes(s) || (i.name || '').toLowerCase().includes(s) || (i.phone || '').toLowerCase().includes(s) || (i.email || '').toLowerCase().includes(s));
    if (this.filterActive() !== null) list = list.filter(i => i.isActive === this.filterActive());
    return list;
  });
  totalCount = computed(() => (this.items() || []).length);
  activeCount = computed(() => (this.items() || []).filter(i => i.isActive).length);

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) {}

  ngOnInit() {
    this.loadData();
  }

  async loadData() {
    try {
      const res: any = await this.api.invoke(apiProvidersGet);
      this.items.set(res?.data || []);
    } catch (err) {
      console.error(err);
    }
  }

  setFilter(val: boolean | null) { this.filterActive.set(this.filterActive() === val ? null : val); }
  openNew() { this.form = { isActive: true, code: '', name: '', phone: '', email: '', taxCode: '', contactPerson: '', address: '', note: '' }; this.autoCode = true; this.isEdit.set(false); this.dialogVisible.set(true); }
  editItem(item: ProviderItem) { this.form = { ...item }; this.isEdit.set(true); this.dialogVisible.set(true); }

  deleteItem(item: ProviderItem) {
    this.confirmationService.confirm({
      message: `Bạn có chắc muốn xóa <b>${item.name}</b>?`, header: 'Xác nhận xóa', icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Xóa', rejectLabel: 'Hủy', acceptButtonStyleClass: 'p-button-danger',
      accept: async () => { 
        try {
          await this.api.invoke(apiProvidersIdDelete, { id: item.id });
          this.items.update(l => l.filter(i => i.id !== item.id)); 
          this.messageService.add({ severity: 'success', summary: 'Đã xóa', detail: `${item.name} đã được xóa.`, life: 3000 }); 
        } catch (err) {
          console.error(err);
        }
      }
    });
  }

  async saveItem() {
    if (!this.form.name) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng nhập tên nhà cung cấp.', life: 3000 }); return; }
    const body = { ...this.form, code: this.autoCode ? '' : (this.form.code || '') };
    try {
      if (this.isEdit()) {
        await this.api.invoke(apiProvidersIdPut, { id: this.form.id, body });
        this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Nhà cung cấp đã được cập nhật.', life: 3000 });
      } else {
        await this.api.invoke(apiProvidersPost, { body });
        this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Nhà cung cấp mới đã được tạo.', life: 3000 });
      }
      this.dialogVisible.set(false);
      this.loadData();
    } catch (err) {
      console.error(err);
    }
  }
}
