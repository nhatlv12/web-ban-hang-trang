import { WareHouse } from './warehouse.model';

/** Sản phẩm */
export interface Product {
  id: string;
  code: string;
  name: string;
  description?: string;
  categoryId: string;
  providerId: string;
  costPrice: number;
  sellingPrice: number;
  originalPrice?: number;
  unit: string;
  image?: string;
  isNew: boolean;
  isSale: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;

  // Navigation (optional, populated khi API include)
  category?: Category;
  wareHouse?: WareHouse;
}

/** Giỏ hàng - item (dùng ở frontend) */
export interface CartItem {
  product: Product;
  quantity: number;
}

/** Danh mục sản phẩm (hỗ trợ đa cấp) */
export interface Category {
  id: string;
  code: string;
  name: string;
  description?: string;
  icon?: string;
  parentId?: string;
  sortOrder: number;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;

  // Navigation (optional)
  children?: Category[];
}

/** Người dùng hệ thống */
export interface User {
  id: string;
  username: string;
  email: string;
  fullName: string;
  avatar?: string;
  role: 'admin' | 'user';
}

/** Response wrapper từ API */
export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
  total?: number;
}
