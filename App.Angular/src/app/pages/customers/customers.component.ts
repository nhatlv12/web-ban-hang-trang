import { Component, signal, computed, OnInit } from '@angular/core';
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

interface CustomerItem {
  id: string; code: string; fullName: string; phone: string; email?: string;
  address?: string; dateOfBirth?: string; gender?: number; note?: string;
  isActive: boolean; createdAt: string;
}

@Component({
  selector: 'app-customers',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, TextareaModule, TagModule, SelectModule, DatePickerModule, TooltipModule, FloatLabelModule],
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
  genderOptions = [{ label: 'Nam', value: 1 }, { label: 'Nữ', value: 2 }, { label: 'Khác', value: 3 }];

  filteredItems = computed(() => {
    let list = this.items();
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => i.code.toLowerCase().includes(s) || i.fullName.toLowerCase().includes(s) || i.phone.includes(s));
    if (this.filterActive() !== null) list = list.filter(i => i.isActive === this.filterActive());
    return list;
  });
  totalCount = computed(() => this.items().length);
  activeCount = computed(() => this.items().filter(i => i.isActive).length);

  constructor(private confirmationService: ConfirmationService, private messageService: MessageService) {}

  ngOnInit() {
    this.items.set([
      { id: '1', code: 'KH001', fullName: 'Nguyễn Văn An', phone: '0901234567', email: 'an@gmail.com', address: 'Q1, TP.HCM', gender: 1, dateOfBirth: '1990-05-15', isActive: true, createdAt: '2026-01-10' },
      { id: '2', code: 'KH002', fullName: 'Trần Thị Bình', phone: '0912345678', email: 'binh@gmail.com', address: 'Q3, TP.HCM', gender: 2, dateOfBirth: '1995-08-20', isActive: true, createdAt: '2026-02-14' },
      { id: '3', code: 'KH003', fullName: 'Lê Hoàng Cường', phone: '0923456789', email: 'cuong@gmail.com', address: 'Hà Nội', gender: 1, isActive: false, createdAt: '2026-03-01' },
      { id: '4', code: 'KH004', fullName: 'Phạm Thị Dung', phone: '0934567890', email: 'dung@gmail.com', address: 'Đà Nẵng', gender: 2, dateOfBirth: '1988-12-01', isActive: true, createdAt: '2026-04-18' },
    ]);
  }

  getGenderLabel(g?: number) { return g === 1 ? 'Nam' : g === 2 ? 'Nữ' : g === 3 ? 'Khác' : '—'; }
  setFilter(v: boolean | null) { this.filterActive.set(this.filterActive() === v ? null : v); }
  openNew() { this.form = { isActive: true, gender: 1, code: '', fullName: '', phone: '', email: '', address: '', note: '' }; this.isEdit.set(false); this.dialogVisible.set(true); }
  editItem(item: CustomerItem) { this.form = { ...item }; this.isEdit.set(true); this.dialogVisible.set(true); }

  deleteItem(item: CustomerItem) {
    this.confirmationService.confirm({
      message: `Bạn có chắc muốn xóa <b>${item.fullName}</b>?`, header: 'Xác nhận xóa', icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Xóa', rejectLabel: 'Hủy', acceptButtonStyleClass: 'p-button-danger',
      accept: () => { this.items.update(l => l.filter(i => i.id !== item.id)); this.messageService.add({ severity: 'success', summary: 'Đã xóa', detail: `${item.fullName} đã được xóa.`, life: 3000 }); }
    });
  }

  saveItem() {
    if (!this.form.code || !this.form.fullName || !this.form.phone) { this.messageService.add({ severity: 'warn', summary: 'Thiếu thông tin', detail: 'Vui lòng nhập mã, họ tên và SĐT.', life: 3000 }); return; }
    if (this.isEdit()) {
      this.items.update(l => l.map(i => i.id === this.form.id ? { ...i, ...this.form } : i));
      this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Khách hàng đã được cập nhật.', life: 3000 });
    } else {
      this.items.update(l => [{ ...this.form, id: crypto.randomUUID(), createdAt: new Date().toISOString().slice(0, 10), isActive: true }, ...l]);
      this.messageService.add({ severity: 'success', summary: 'Đã tạo', detail: 'Khách hàng mới đã được tạo.', life: 3000 });
    }
    this.dialogVisible.set(false);
  }
}
