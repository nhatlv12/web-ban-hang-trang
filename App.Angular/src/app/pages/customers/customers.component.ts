import { Component, signal, computed, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { TagModule } from 'primeng/tag';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { TooltipModule } from 'primeng/tooltip';
import { FloatLabelModule } from 'primeng/floatlabel';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Api } from '../../api/api';
import { apiCustomersGet, apiCustomersPost, apiCustomersIdPut, apiCustomersIdDelete } from '../../api/functions';
import { SearchableSelectComponent } from '../../shared/components/searchable-select/searchable-select.component';

interface CustomerItem {
  id: string; code: string; fullName: string; phone: string; email?: string;
  address?: string; dateOfBirth?: string; gender?: number; note?: string;
  isActive: boolean; createdAt: string;
}

@Component({
  selector: 'app-customers',
  imports: [CommonModule, FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, TextareaModule, TagModule, SelectModule, DatePickerModule, TooltipModule, FloatLabelModule, SearchableSelectComponent],
  templateUrl: './customers.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class Customers implements OnInit {
  items = signal<CustomerItem[]>([]);
  dialogVisible = signal(false);
  isEdit = signal(false);
  searchValue = signal('');
  filterActive = signal<boolean | null>(null);
  form: any = {};
  autoCode = true;
  genderOptions = [{ label: 'Nam', value: 1 }, { label: 'Nữ', value: 2 }, { label: 'Khác', value: 3 }];

  private api = inject(Api);

  filteredItems = computed(() => {
    let list = this.items() || [];
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => (i.code || '').toLowerCase().includes(s) || (i.fullName || '').toLowerCase().includes(s) || (i.phone || '').includes(s));
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
      const res: any = await this.api.invoke(apiCustomersGet);
      this.items.set(res?.data || []);
    } catch (err) {
      console.error(err);
    }
  }

  getGenderLabel(g?: number) { return g === 1 ? 'Nam' : g === 2 ? 'Nữ' : g === 3 ? 'Khác' : '—'; }
  setFilter(v: boolean | null) { this.filterActive.set(this.filterActive() === v ? null : v); }
  openNew() { this.form = { isActive: true, gender: 1, code: '', fullName: '', phone: '', email: '', address: '', note: '' }; this.autoCode = true; this.isEdit.set(false); this.dialogVisible.set(true); }
  editItem(item: CustomerItem) { 
    this.form = { ...item }; 
    if (this.form.dateOfBirth) {
      this.form.dateOfBirth = new Date(this.form.dateOfBirth);
    }
    this.isEdit.set(true); 
    this.dialogVisible.set(true); 
  }

  deleteItem(item: CustomerItem) {
    this.confirmationService.confirm({
      message: `Bạn có chắc muốn xóa <b>${item.fullName}</b>?`, header: 'Xác nhận xóa', icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Xóa', rejectLabel: 'Hủy', acceptButtonStyleClass: 'p-button-danger',
      accept: async () => { 
        try {
          await this.api.invoke(apiCustomersIdDelete, { id: item.id });
          this.items.update(l => l.filter(i => i.id !== item.id)); 
          this.messageService.add({ severity: 'success', summary: 'Đã xóa', detail: `${item.fullName} đã được xóa.`, life: 3000 }); 
        } catch (err) {
          console.error(err);
        }
      }
    });
  }

  async saveItem() {
    if (!this.form.fullName || !this.form.phone) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng nhập họ tên và SĐT.', life: 3000 }); return; }
    try {
      const payload = { ...this.form, code: this.autoCode ? '' : (this.form.code || '') };
      if (payload.dateOfBirth && payload.dateOfBirth instanceof Date) {
        const d = payload.dateOfBirth;
        payload.dateOfBirth = new Date(d.getTime() - (d.getTimezoneOffset() * 60000)).toISOString().split('T')[0];
      }

      if (this.isEdit()) {
        await this.api.invoke(apiCustomersIdPut, { id: payload.id, body: payload });
        this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Khách hàng đã được cập nhật.', life: 3000 });
      } else {
        await this.api.invoke(apiCustomersPost, { body: payload });
        this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Khách hàng mới đã được tạo.', life: 3000 });
      }
      this.dialogVisible.set(false);
      this.loadData();
    } catch (err) {
      console.error(err);
    }
  }
}
