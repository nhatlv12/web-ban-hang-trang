import { Injectable, signal } from '@angular/core';
import { Product, Category } from '../models/product.model';
import { ApiService } from './api.service';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  constructor(private api: ApiService) {}

  /** Dữ liệu mẫu - thay bằng API thật khi có backend */
  private readonly mockProducts: Product[] = [
    {
      id: 1, name: 'iPhone 16 Pro Max', description: 'Điện thoại Apple iPhone 16 Pro Max 256GB',
      price: 34990000, originalPrice: 38990000, image: 'https://placehold.co/300x300/4F46E5/white?text=iPhone+16',
      category: 'Điện thoại', rating: 4.8, stock: 50, isNew: true, isSale: true
    },
    {
      id: 2, name: 'Samsung Galaxy S25 Ultra', description: 'Điện thoại Samsung Galaxy S25 Ultra 512GB',
      price: 31990000, originalPrice: 35990000, image: 'https://placehold.co/300x300/7C3AED/white?text=Galaxy+S25',
      category: 'Điện thoại', rating: 4.7, stock: 35, isNew: true, isSale: true
    },
    {
      id: 3, name: 'MacBook Pro M4', description: 'Laptop Apple MacBook Pro 14 inch M4 Pro 24GB 512GB',
      price: 49990000, image: 'https://placehold.co/300x300/1E40AF/white?text=MacBook+M4',
      category: 'Laptop', rating: 4.9, stock: 20, isNew: true
    },
    {
      id: 4, name: 'AirPods Pro 3', description: 'Tai nghe Apple AirPods Pro thế hệ 3',
      price: 6990000, originalPrice: 7990000, image: 'https://placehold.co/300x300/059669/white?text=AirPods+3',
      category: 'Phụ kiện', rating: 4.6, stock: 100, isSale: true
    },
    {
      id: 5, name: 'iPad Air M3', description: 'Máy tính bảng Apple iPad Air M3 11 inch 128GB',
      price: 18990000, image: 'https://placehold.co/300x300/DC2626/white?text=iPad+Air',
      category: 'Tablet', rating: 4.5, stock: 40, isNew: true
    },
    {
      id: 6, name: 'Apple Watch Ultra 3', description: 'Đồng hồ thông minh Apple Watch Ultra 3 GPS + Cellular',
      price: 22990000, originalPrice: 24990000, image: 'https://placehold.co/300x300/EA580C/white?text=Watch+Ultra',
      category: 'Phụ kiện', rating: 4.7, stock: 25, isSale: true
    },
    {
      id: 7, name: 'Dell XPS 15', description: 'Laptop Dell XPS 15 Core Ultra 9 32GB 1TB',
      price: 42990000, image: 'https://placehold.co/300x300/0891B2/white?text=Dell+XPS',
      category: 'Laptop', rating: 4.4, stock: 15
    },
    {
      id: 8, name: 'Sony WH-1000XM6', description: 'Tai nghe chống ồn Sony WH-1000XM6',
      price: 8490000, originalPrice: 9990000, image: 'https://placehold.co/300x300/6D28D9/white?text=Sony+XM6',
      category: 'Phụ kiện', rating: 4.8, stock: 60, isSale: true
    }
  ];

  private readonly mockCategories: Category[] = [
    { id: 1, name: 'Điện thoại', icon: 'pi pi-mobile', count: 120 },
    { id: 2, name: 'Laptop', icon: 'pi pi-desktop', count: 85 },
    { id: 3, name: 'Tablet', icon: 'pi pi-tablet', count: 45 },
    { id: 4, name: 'Phụ kiện', icon: 'pi pi-headphones', count: 230 },
  ];

  getProducts(): Observable<Product[]> {
    // TODO: Thay bằng API thật: return this.api.get<Product[]>('products');
    return of(this.mockProducts);
  }

  getProductById(id: number): Observable<Product | undefined> {
    return of(this.mockProducts.find(p => p.id === id));
  }

  getCategories(): Observable<Category[]> {
    return of(this.mockCategories);
  }

  getProductsByCategory(category: string): Observable<Product[]> {
    return of(this.mockProducts.filter(p => p.category === category));
  }

  searchProducts(keyword: string): Observable<Product[]> {
    const filtered = this.mockProducts.filter(p =>
      p.name.toLowerCase().includes(keyword.toLowerCase()) ||
      p.description.toLowerCase().includes(keyword.toLowerCase())
    );
    return of(filtered);
  }
}
