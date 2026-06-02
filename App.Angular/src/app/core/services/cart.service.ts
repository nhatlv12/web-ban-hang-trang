import { Injectable, signal, computed } from '@angular/core';
import { Product, CartItem } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly cartItems = signal<CartItem[]>([]);

  /** Danh sách sản phẩm trong giỏ hàng */
  readonly items = this.cartItems.asReadonly();

  /** Tổng số lượng sản phẩm */
  readonly totalQuantity = computed(() =>
    this.cartItems().reduce((sum, item) => sum + item.quantity, 0)
  );

  /** Tổng tiền */
  readonly totalPrice = computed(() =>
    this.cartItems().reduce((sum, item) => sum + item.product.sellingPrice * item.quantity, 0)
  );

  /** Số loại sản phẩm */
  readonly itemCount = computed(() => this.cartItems().length);

  addToCart(product: Product, quantity: number = 1): void {
    const currentItems = this.cartItems();
    const existingIndex = currentItems.findIndex(item => item.product.id === product.id);

    if (existingIndex >= 0) {
      const updatedItems = [...currentItems];
      updatedItems[existingIndex] = {
        ...updatedItems[existingIndex],
        quantity: updatedItems[existingIndex].quantity + quantity
      };
      this.cartItems.set(updatedItems);
    } else {
      this.cartItems.set([...currentItems, { product, quantity }]);
    }
  }

  removeFromCart(productId: string): void {
    this.cartItems.update(items => items.filter(item => item.product.id !== productId));
  }

  updateQuantity(productId: string, quantity: number): void {
    if (quantity <= 0) {
      this.removeFromCart(productId);
      return;
    }
    this.cartItems.update(items =>
      items.map(item =>
        item.product.id === productId ? { ...item, quantity } : item
      )
    );
  }

  clearCart(): void {
    this.cartItems.set([]);
  }

  isInCart(productId: string): boolean {
    return this.cartItems().some(item => item.product.id === productId);
  }
}
