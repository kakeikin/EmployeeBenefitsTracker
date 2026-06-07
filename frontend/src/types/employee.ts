export type EmploymentStatus = 'Active' | 'Inactive';

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  department: string;
  employmentStatus: EmploymentStatus;
  createdAt: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  department: string;
  employmentStatus: EmploymentStatus;
}

export type UpdateEmployeeRequest = CreateEmployeeRequest;

export interface EmployeeFilters {
  page?: number;
  pageSize?: number;
  search?: string;
  department?: string;
  benefitStatus?: string;
}
