/** Kho hàng - quản lý tồn kho */
export interface WareHouse {
  id: string;
  productId: string;
  quantity: number;
  minQuantity: number;
  maxQuantity: number;
  location?: string;
  lastStockUpdate: string;
  createdAt: string;
  updatedAt?: string;
}
