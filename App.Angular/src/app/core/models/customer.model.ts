/** Giới tính */
export enum Gender {
  Male = 1,
  Female = 2,
  Other = 3
}

/** Khách hàng mua hàng */
export interface Customer {
  id: string;
  code: string;
  fullName: string;
  phone: string;
  email?: string;
  address?: string;
  dateOfBirth?: string;
  gender?: Gender;
  note?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}
