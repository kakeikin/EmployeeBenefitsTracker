import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type { CreateEmployeeRequest, Employee, EmployeeFilters, PagedResult, UpdateEmployeeRequest } from '../types/employee';
import apiClient from './axios';

const EMPLOYEES_KEY = 'employees';

export const useEmployees = (filters: EmployeeFilters = {}) =>
  useQuery<PagedResult<Employee>>({
    queryKey: [EMPLOYEES_KEY, filters],
    queryFn: async () => {
      const params = new URLSearchParams();
      if (filters.page) params.set('page', String(filters.page));
      if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
      if (filters.search) params.set('search', filters.search);
      if (filters.department) params.set('department', filters.department);
      if (filters.benefitStatus) params.set('benefitStatus', filters.benefitStatus);
      const { data } = await apiClient.get(`/api/employees?${params}`);
      return data;
    },
  });

export const useEmployee = (id: number) =>
  useQuery<Employee>({
    queryKey: [EMPLOYEES_KEY, id],
    queryFn: async () => {
      const { data } = await apiClient.get(`/api/employees/${id}`);
      return data;
    },
    enabled: id > 0,
  });

export const useCreateEmployee = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (dto: CreateEmployeeRequest) => {
      const { data } = await apiClient.post<Employee>('/api/employees', dto);
      return data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: [EMPLOYEES_KEY] }),
  });
};

export const useUpdateEmployee = (id: number) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (dto: UpdateEmployeeRequest) => {
      const { data } = await apiClient.put<Employee>(`/api/employees/${id}`, dto);
      return data;
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: [EMPLOYEES_KEY] });
      qc.invalidateQueries({ queryKey: [EMPLOYEES_KEY, id] });
    },
  });
};

export const useDeleteEmployee = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => { await apiClient.delete(`/api/employees/${id}`); },
    onSuccess: () => qc.invalidateQueries({ queryKey: [EMPLOYEES_KEY] }),
  });
};
