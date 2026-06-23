import { Component, Input, Output, EventEmitter, forwardRef, ElementRef, HostListener, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NG_VALUE_ACCESSOR, ControlValueAccessor } from '@angular/forms';

export interface SelectOption {
  label: string;
  value: any;
}

@Component({
  selector: 'app-searchable-select',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => SearchableSelectComponent),
    multi: true
  }],
  template: `
    <div class="ss-wrapper" [class.ss-open]="isOpen()" [class.ss-has-value]="!!selectedValue()">
      <div class="ss-trigger" (click)="toggle()" [attr.id]="inputId">
        <span class="ss-value" [class.ss-placeholder]="!selectedLabel()">
          {{ selectedLabel() || placeholder }}
        </span>
        <span class="ss-arrow">
          <i class="pi pi-chevron-down"></i>
        </span>
      </div>

      @if (isOpen()) {
        <div class="ss-dropdown">
          @if (searchable) {
            <div class="ss-search-box">
              <i class="pi pi-search"></i>
              <input
                type="text"
                class="ss-search-input"
                placeholder="Tìm kiếm..."
                [ngModel]="searchTerm()"
                (ngModelChange)="searchTerm.set($event)"
                (click)="$event.stopPropagation()"
                #searchInput
              />
              @if (searchTerm()) {
                <i class="pi pi-times ss-clear-search" (click)="clearSearch($event)"></i>
              }
            </div>
          }
          <div class="ss-options-list">
            @if (showClear && selectedValue() != null) {
              <div class="ss-option ss-option-clear" (click)="selectOption(null, $event)">
                <i class="pi pi-times-circle"></i> Bỏ chọn
              </div>
            }
            @for (opt of filteredOptions(); track opt.value) {
              <div
                class="ss-option"
                [class.ss-selected]="opt.value === selectedValue()"
                (click)="selectOption(opt, $event)"
              >
                <span class="ss-option-label">{{ opt.label }}</span>
                @if (opt.value === selectedValue()) {
                  <i class="pi pi-check ss-check-icon"></i>
                }
              </div>
            } @empty {
              <div class="ss-no-results">
                <i class="pi pi-search"></i>
                <span>Không tìm thấy kết quả</span>
              </div>
            }
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
      position: relative;
    }

    .ss-wrapper {
      position: relative;
      width: 100%;
    }

    .ss-trigger {
      display: flex;
      align-items: center;
      justify-content: space-between;
      width: 100%;
      min-height: 44px;
      padding: 0.7rem 0.85rem;
      border: 1.5px solid #e2e8f0;
      border-radius: 10px;
      background: #fff;
      cursor: pointer;
      transition: all 0.2s ease;
      font-size: 0.93rem;
      color: #334155;
      gap: 0.5rem;
    }

    .ss-trigger:hover {
      border-color: #a7f3d0;
    }

    .ss-open .ss-trigger {
      border-color: #10b981;
      box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.12);
    }

    .ss-value {
      flex: 1;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    .ss-placeholder {
      color: #94a3b8;
    }

    .ss-arrow {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 20px;
      flex-shrink: 0;
      transition: transform 0.25s cubic-bezier(0.4, 0, 0.2, 1);
    }

    .ss-arrow i {
      font-size: 0.75rem;
      color: #94a3b8;
      transition: color 0.2s ease;
    }

    .ss-open .ss-arrow {
      transform: rotate(180deg);
    }

    .ss-open .ss-arrow i {
      color: #10b981;
    }

    /* ===== Dropdown Panel ===== */
    .ss-dropdown {
      position: absolute;
      top: calc(100% + 6px);
      left: 0;
      right: 0;
      background: #fff;
      border: 1.5px solid #d1fae5;
      border-radius: 12px;
      box-shadow: 0 12px 40px rgba(0, 0, 0, 0.1), 0 4px 12px rgba(16, 185, 129, 0.08);
      z-index: 1050;
      overflow: hidden;
      animation: ssSlideDown 0.2s cubic-bezier(0.4, 0, 0.2, 1);
    }

    @keyframes ssSlideDown {
      from {
        opacity: 0;
        transform: translateY(-8px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }

    /* ===== Search ===== */
    .ss-search-box {
      position: relative;
      padding: 0.65rem;
      border-bottom: 1px solid #ecfdf5;
    }

    .ss-search-box > i.pi-search {
      position: absolute;
      left: 1.15rem;
      top: 50%;
      transform: translateY(-50%);
      font-size: 0.8rem;
      color: #94a3b8;
    }

    .ss-search-input {
      width: 100%;
      border: 1.5px solid #e2e8f0;
      border-radius: 8px;
      padding: 0.5rem 2rem 0.5rem 2.1rem;
      font-size: 0.87rem;
      color: #334155;
      background: #f8fafc;
      outline: none;
      transition: all 0.2s ease;
      font-family: inherit;
    }

    .ss-search-input:focus {
      border-color: #10b981;
      background: #fff;
      box-shadow: 0 0 0 2px rgba(16, 185, 129, 0.08);
    }

    .ss-search-input::placeholder {
      color: #cbd5e1;
    }

    .ss-clear-search {
      position: absolute;
      right: 1.15rem;
      top: 50%;
      transform: translateY(-50%);
      font-size: 0.75rem;
      color: #94a3b8;
      cursor: pointer;
      padding: 4px;
      border-radius: 50%;
      transition: all 0.15s ease;
    }

    .ss-clear-search:hover {
      color: #ef4444;
      background: #fef2f2;
    }

    /* ===== Options ===== */
    .ss-options-list {
      max-height: 220px;
      overflow-y: auto;
      padding: 0.35rem;
    }

    .ss-options-list::-webkit-scrollbar {
      width: 5px;
    }

    .ss-options-list::-webkit-scrollbar-track {
      background: transparent;
    }

    .ss-options-list::-webkit-scrollbar-thumb {
      background: #d1fae5;
      border-radius: 3px;
    }

    .ss-option {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0.6rem 0.8rem;
      border-radius: 8px;
      cursor: pointer;
      font-size: 0.9rem;
      color: #475569;
      transition: all 0.15s ease;
      gap: 0.5rem;
    }

    .ss-option:hover {
      background: #ecfdf5;
      color: #059669;
    }

    .ss-option.ss-selected {
      background: linear-gradient(135deg, #ecfdf5, #d1fae5);
      color: #059669;
      font-weight: 600;
    }

    .ss-option.ss-option-clear {
      color: #94a3b8;
      font-size: 0.85rem;
      border-bottom: 1px solid #f1f5f9;
      border-radius: 8px 8px 0 0;
      margin-bottom: 0.2rem;
    }

    .ss-option.ss-option-clear:hover {
      color: #ef4444;
      background: #fef2f2;
    }

    .ss-option.ss-option-clear i {
      font-size: 0.8rem;
    }

    .ss-check-icon {
      font-size: 0.75rem;
      color: #10b981;
      flex-shrink: 0;
    }

    .ss-option-label {
      flex: 1;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    /* ===== No Results ===== */
    .ss-no-results {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 0.5rem;
      padding: 1.5rem 1rem;
      color: #94a3b8;
      font-size: 0.85rem;
    }

    .ss-no-results i {
      font-size: 1.5rem;
      opacity: 0.3;
    }
  `]
})
export class SearchableSelectComponent implements ControlValueAccessor {
  @Input() options: SelectOption[] = [];
  @Input() placeholder = 'Chọn...';
  @Input() searchable = true;
  @Input() showClear = false;
  @Input() inputId = '';

  isOpen = signal(false);
  searchTerm = signal('');
  selectedValue = signal<any>(null);

  private onChange: (val: any) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(private elRef: ElementRef) {}

  selectedLabel = computed(() => {
    const val = this.selectedValue();
    if (val == null) return '';
    const opt = this.options.find(o => o.value === val || o.value?.toString() === val?.toString());
    return opt?.label || '';
  });

  filteredOptions = computed(() => {
    const term = this.searchTerm().toLowerCase().trim();
    if (!term) return this.options;
    return this.options.filter(o => o.label.toLowerCase().includes(term));
  });

  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event) {
    if (!this.elRef.nativeElement.contains(event.target)) {
      this.close();
    }
  }

  toggle() {
    if (this.isOpen()) {
      this.close();
    } else {
      this.isOpen.set(true);
      this.searchTerm.set('');
      // Auto-focus search input
      setTimeout(() => {
        const input = this.elRef.nativeElement.querySelector('.ss-search-input');
        input?.focus();
      }, 50);
    }
  }

  close() {
    this.isOpen.set(false);
    this.searchTerm.set('');
    this.onTouched();
  }

  selectOption(opt: SelectOption | null, event: Event) {
    event.stopPropagation();
    const val = opt ? opt.value : null;
    this.selectedValue.set(val);
    this.onChange(val);
    this.close();
  }

  clearSearch(event: Event) {
    event.stopPropagation();
    this.searchTerm.set('');
    const input = this.elRef.nativeElement.querySelector('.ss-search-input');
    input?.focus();
  }

  // ControlValueAccessor
  writeValue(val: any): void {
    this.selectedValue.set(val);
  }
  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }
}
