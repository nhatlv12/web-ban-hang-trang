import { Customer } from './customer.model';
import { Provider } from './provider.model';
import { Product } from './product.model';

/** Loại đơn hàng */
export enum OrderType {
  /** Nhập hàng (từ nhà cung cấp) */
  Import = 1,
  /** Xuất hàng (bán cho khách hàng) */
  Export = 2
}

/** Trạng thái đơn hàng */
export enum OrderStatus {
  /** Nháp */
  Draft = 0,
  /** Chờ xử lý */
  Pending = 1,
  /** Đã xác nhận */
  Confirmed = 2,
  /** Hoàn thành */
  Completed = 3,
  /** Đã hủy */
  Cancelled = 4
}

/** Đơn hàng (phiếu nhập/xuất) */
export interface Order {
  id: string;
  code: string;
  type: OrderType;
  status: OrderStatus;
  orderDate: string;
  providerId?: string;
  customerId?: string;
  totalAmount: number;
  discount: number;
  finalAmount: number;
  note?: string;
  createdBy?: string;
  createdAt: string;
  updatedAt?: string;

  // Navigation (optional, populated khi API include)
  provider?: Provider;
  customer?: Customer;
  orderDetails?: OrderDetail[];
}

/** Chi tiết đơn hàng */
export interface OrderDetail {
  id: string;
  orderId: string;
  productId: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  totalPrice: number;

  // Navigation (optional)
  product?: Product;
}
