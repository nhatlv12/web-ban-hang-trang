export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  originalPrice?: number;
  image: string;
  category: string;
  rating: number;
  stock: number;
  isNew?: boolean;
  isSale?: boolean;
}

export interface CartItem {
  product: Product;
  quantity: number;
}

export interface Category {
  id: number;
  name: string;
  icon: string;
  count: number;
}

export interface User {
  id: number;
  username: string;
  email: string;
  fullName: string;
  avatar?: string;
  role: 'admin' | 'user';
}

export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
  total?: number;
}
