import { Injectable } from '@angular/core';
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
      id: '1', code: 'SP001', name: 'iPhone 16 Pro Max', description: 'Điện thoại Apple iPhone 16 Pro Max 256GB',
      sellingPrice: 34990000, originalPrice: 38990000, costPrice: 30000000,
      image: 'https://placehold.co/300x300/4F46E5/white?text=iPhone+16',
      categoryId: '1', providerId: '1', unit: 'Cái', isNew: true, isSale: true, isActive: true,
      createdAt: new Date().toISOString()
    },
    {
      id: '2', code: 'SP002', name: 'Samsung Galaxy S25 Ultra', description: 'Điện thoại Samsung Galaxy S25 Ultra 512GB',
      sellingPrice: 31990000, originalPrice: 35990000, costPrice: 27000000,
      image: 'https://placehold.co/300x300/7C3AED/white?text=Galaxy+S25',
      categoryId: '1', providerId: '2', unit: 'Cái', isNew: true, isSale: true, isActive: true,
      createdAt: new Date().toISOString()
    },
    {
      id: '3', code: 'SP003', name: 'MacBook Pro M4', description: 'Laptop Apple MacBook Pro 14 inch M4 Pro 24GB 512GB',
      sellingPrice: 49990000, costPrice: 43000000,
      image: 'https://placehold.co/300x300/1E40AF/white?text=MacBook+M4',
      categoryId: '2', providerId: '1', unit: 'Cái', isNew: true, isSale: false, isActive: true,
      createdAt: new Date().toISOString()
    },
    {
      id: '4', code: 'SP004', name: 'AirPods Pro 3', description: 'Tai nghe Apple AirPods Pro thế hệ 3',
      sellingPrice: 6990000, originalPrice: 7990000, costPrice: 5500000,
      image: 'https://placehold.co/300x300/059669/white?text=AirPods+3',
      categoryId: '4', providerId: '1', unit: 'Cái', isNew: false, isSale: true, isActive: true,
      createdAt: new Date().toISOString()
    },
    {
      id: '5', code: 'SP005', name: 'iPad Air M3', description: 'Máy tính bảng Apple iPad Air M3 11 inch 128GB',
      sellingPrice: 18990000, costPrice: 16000000,
      image: 'https://placehold.co/300x300/DC2626/white?text=iPad+Air',
      categoryId: '3', providerId: '1', unit: 'Cái', isNew: true, isSale: false, isActive: true,
      createdAt: new Date().toISOString()
    },
    {
      id: '6', code: 'SP006', name: 'Apple Watch Ultra 3', description: 'Đồng hồ thông minh Apple Watch Ultra 3 GPS + Cellular',
      sellingPrice: 22990000, originalPrice: 24990000, costPrice: 19000000,
      image: 'https://placehold.co/300x300/EA580C/white?text=Watch+Ultra',
      categoryId: '4', providerId: '1', unit: 'Cái', isNew: false, isSale: true, isActive: true,
      createdAt: new Date().toISOString()
    },
    {
      id: '7', code: 'SP007', name: 'Dell XPS 15', description: 'Laptop Dell XPS 15 Core Ultra 9 32GB 1TB',
      sellingPrice: 42990000, costPrice: 37000000,
      image: 'https://placehold.co/300x300/0891B2/white?text=Dell+XPS',
      categoryId: '2', providerId: '3', unit: 'Cái', isNew: false, isSale: false, isActive: true,
      createdAt: new Date().toISOString()
    },
    {
      id: '8', code: 'SP008', name: 'Sony WH-1000XM6', description: 'Tai nghe chống ồn Sony WH-1000XM6',
      sellingPrice: 8490000, originalPrice: 9990000, costPrice: 6500000,
      image: 'https://placehold.co/300x300/6D28D9/white?text=Sony+XM6',
      categoryId: '4', providerId: '4', unit: 'Cái', isNew: false, isSale: true, isActive: true,
      createdAt: new Date().toISOString()
    }
  ];

  private readonly mockCategories: Category[] = [
    { id: '1', code: 'DM001', name: 'Điện thoại', icon: 'pi pi-mobile', sortOrder: 1, isActive: true, createdAt: new Date().toISOString() },
    { id: '2', code: 'DM002', name: 'Laptop', icon: 'pi pi-desktop', sortOrder: 2, isActive: true, createdAt: new Date().toISOString() },
    { id: '3', code: 'DM003', name: 'Tablet', icon: 'pi pi-tablet', sortOrder: 3, isActive: true, createdAt: new Date().toISOString() },
    { id: '4', code: 'DM004', name: 'Phụ kiện', icon: 'pi pi-headphones', sortOrder: 4, isActive: true, createdAt: new Date().toISOString() },
  ];

  getProducts(): Observable<Product[]> {
    // TODO: Thay bằng API thật: return this.api.get<Product[]>('products');
    return of(this.mockProducts);
  }

  getProductById(id: string): Observable<Product | undefined> {
    return of(this.mockProducts.find(p => p.id === id));
  }

  getCategories(): Observable<Category[]> {
    return of(this.mockCategories);
  }

  getProductsByCategory(categoryId: string): Observable<Product[]> {
    return of(this.mockProducts.filter(p => p.categoryId === categoryId));
  }

  searchProducts(keyword: string): Observable<Product[]> {
    const filtered = this.mockProducts.filter(p =>
      p.name.toLowerCase().includes(keyword.toLowerCase()) ||
      (p.description?.toLowerCase().includes(keyword.toLowerCase()) ?? false)
    );
    return of(filtered);
  }
}
