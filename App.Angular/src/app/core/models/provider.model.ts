/** Nhà cung cấp hàng hóa */
export interface Provider {
  id: string;
  code: string;
  name: string;
  phone?: string;
  email?: string;
  address?: string;
  taxCode?: string;
  contactPerson?: string;
  note?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}
