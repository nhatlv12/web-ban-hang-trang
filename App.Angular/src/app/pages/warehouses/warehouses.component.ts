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
import { MessageService } from 'primeng/api';
import { Api } from '../../api/api';
import { apiWarehousesGet, apiWarehousesIdPut } from '../../api/functions';
import { SearchableSelectComponent } from '../../shared/components/searchable-select/searchable-select.component';

interface WareHouseItem {
  id: string; productId: string; productCode: string; productName: string;
  providerId?: string; providerName?: string;
  quantity: number; minQuantity: number; maxQuantity: number; location?: string;
  lastStockUpdate: string;
}

@Component({
  selector: 'app-warehouses',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, SelectModule, TagModule, TooltipModule, FloatLabelModule, SearchableSelectComponent],
  templateUrl: './warehouses.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class WareHouses implements OnInit {
  items = signal<WareHouseItem[]>([]);
  dialogVisible = signal(false);
  searchValue = signal('');
  filterStock = signal<string | null>(null);
  form: any = {};

  providerOptions = [{ label: 'Apple Việt Nam', value: '1' }, { label: 'Samsung Electronics', value: '2' }, { label: 'Dell Technologies', value: '3' }, { label: 'Sony Vietnam', value: '4' }];

  private api = inject(Api);

  filteredItems = computed(() => {
    let list = this.items() || [];
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => (i.productCode || '').toLowerCase().includes(s) || (i.productName || '').toLowerCase().includes(s) || (i.location || '').toLowerCase().includes(s));
    const f = this.filterStock();
    if (f === 'out') list = list.filter(i => (i.quantity || 0) <= 0);
    else if (f === 'low') list = list.filter(i => (i.quantity || 0) > 0 && (i.quantity || 0) <= (i.minQuantity || 0));
    else if (f === 'ok') list = list.filter(i => (i.quantity || 0) > (i.minQuantity || 0));
    return list;
  });
  totalProducts = computed(() => (this.items() || []).length);
  totalStock = computed(() => (this.items() || []).reduce((sum, i) => sum + (i.quantity || 0), 0));
  lowStockCount = computed(() => (this.items() || []).filter(i => (i.quantity || 0) > 0 && (i.quantity || 0) <= (i.minQuantity || 0)).length);
  outOfStockCount = computed(() => (this.items() || []).filter(i => (i.quantity || 0) <= 0).length);

  constructor(private messageService: MessageService) {}

  ngOnInit() {
    this.loadData();
  }

  async loadData() {
    try {
      const res: any = await this.api.invoke(apiWarehousesGet);
      this.items.set(res?.data || []);
    } catch (err) {
      console.error(err);
    }
  }

  getStockSeverity(item: WareHouseItem): 'success' | 'warn' | 'danger' { return (item.quantity || 0) <= 0 ? 'danger' : (item.quantity || 0) <= (item.minQuantity || 0) ? 'warn' : 'success'; }
  getStockLabel(item: WareHouseItem): string { return (item.quantity || 0) <= 0 ? 'Hết hàng' : (item.quantity || 0) <= (item.minQuantity || 0) ? 'Sắp hết' : 'Đủ hàng'; }
  setFilter(v: string | null) { this.filterStock.set(this.filterStock() === v ? null : v); }
  editItem(item: WareHouseItem) { this.form = { ...item }; this.dialogVisible.set(true); }

  async saveItem() {
    try {
      await this.api.invoke(apiWarehousesIdPut, { id: this.form.id, body: this.form });
      this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Tồn kho đã được cập nhật.', life: 3000 });
      this.dialogVisible.set(false);
      this.loadData();
    } catch (err) {
      console.error(err);
    }
  }
}
