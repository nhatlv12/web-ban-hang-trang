import { Component, signal, computed, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { FloatLabelModule } from 'primeng/floatlabel';
import { MessageService } from 'primeng/api';

interface WareHouseItem {
  id: string; productId: string; productCode: string; productName: string;
  quantity: number; minQuantity: number; maxQuantity: number; location?: string;
  lastStockUpdate: string;
}

@Component({
  selector: 'app-warehouses',
  imports: [FormsModule, TableModule, ButtonModule, DialogModule, InputTextModule, InputNumberModule, TagModule, TooltipModule, FloatLabelModule],
  templateUrl: './warehouses.component.html',
  styleUrls: ['../../shared/styles/crud-page.scss']
})
export class WareHouses implements OnInit {
  items = signal<WareHouseItem[]>([]);
  dialogVisible = signal(false);
  searchValue = signal('');
  filterStock = signal<string | null>(null);
  form: any = {};

  filteredItems = computed(() => {
    let list = this.items();
    const s = this.searchValue().toLowerCase();
    if (s) list = list.filter(i => i.productCode.toLowerCase().includes(s) || i.productName.toLowerCase().includes(s) || i.location?.toLowerCase().includes(s));
    const f = this.filterStock();
    if (f === 'out') list = list.filter(i => i.quantity <= 0);
    else if (f === 'low') list = list.filter(i => i.quantity > 0 && i.quantity <= i.minQuantity);
    else if (f === 'ok') list = list.filter(i => i.quantity > i.minQuantity);
    return list;
  });
  totalProducts = computed(() => this.items().length);
  totalStock = computed(() => this.items().reduce((sum, i) => sum + i.quantity, 0));
  lowStockCount = computed(() => this.items().filter(i => i.quantity > 0 && i.quantity <= i.minQuantity).length);
  outOfStockCount = computed(() => this.items().filter(i => i.quantity <= 0).length);

  constructor(private messageService: MessageService) {}

  ngOnInit() {
    this.items.set([
      { id: '1', productId: '1', productCode: 'SP001', productName: 'iPhone 16 Pro Max 256GB', quantity: 50, minQuantity: 10, maxQuantity: 200, location: 'Kệ A1-01', lastStockUpdate: '2026-06-01' },
      { id: '2', productId: '2', productCode: 'SP002', productName: 'MacBook Pro M4 14"', quantity: 20, minQuantity: 5, maxQuantity: 50, location: 'Kệ B2-03', lastStockUpdate: '2026-06-01' },
      { id: '3', productId: '3', productCode: 'SP003', productName: 'AirPods Pro 3', quantity: 3, minQuantity: 10, maxQuantity: 300, location: 'Kệ C1-02', lastStockUpdate: '2026-05-28' },
      { id: '4', productId: '4', productCode: 'SP004', productName: 'Galaxy S25 Ultra', quantity: 35, minQuantity: 10, maxQuantity: 100, location: 'Kệ A2-01', lastStockUpdate: '2026-05-30' },
      { id: '5', productId: '5', productCode: 'SP005', productName: 'iPad Pro M4 11"', quantity: 0, minQuantity: 5, maxQuantity: 30, location: 'Kệ B1-02', lastStockUpdate: '2026-05-25' },
    ]);
  }

  getStockSeverity(item: WareHouseItem): 'success' | 'warn' | 'danger' { return item.quantity <= 0 ? 'danger' : item.quantity <= item.minQuantity ? 'warn' : 'success'; }
  getStockLabel(item: WareHouseItem): string { return item.quantity <= 0 ? 'Hết hàng' : item.quantity <= item.minQuantity ? 'Sắp hết' : 'Đủ hàng'; }
  setFilter(v: string | null) { this.filterStock.set(this.filterStock() === v ? null : v); }
  editItem(item: WareHouseItem) { this.form = { ...item }; this.dialogVisible.set(true); }

  saveItem() {
    this.items.update(l => l.map(i => i.id === this.form.id ? { ...i, ...this.form, lastStockUpdate: new Date().toISOString().slice(0, 10) } : i));
    this.messageService.add({ severity: 'success', summary: 'Đã cập nhật', detail: 'Tồn kho đã được cập nhật.', life: 3000 });
    this.dialogVisible.set(false);
  }
}
