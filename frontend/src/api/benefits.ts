import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type { BenefitEnrollment, BenefitSummary, CreateBenefitRequest, UpdateBenefitRequest } from '../types/benefit';
import apiClient from './axios';

const BENEFITS_KEY = 'benefits';
const SUMMARY_KEY = 'benefit-summary';

export const useEmployeeBenefits = (employeeId: number) =>
  useQuery<BenefitEnrollment[]>({
    queryKey: [BENEFITS_KEY, employeeId],
    queryFn: async () => {
      const { data } = await apiClient.get(`/api/employees/${employeeId}/benefits`);
      return data;
    },
    enabled: employeeId > 0,
  });

export const useBenefitSummary = () =>
  useQuery<BenefitSummary>({
    queryKey: [SUMMARY_KEY],
    queryFn: async () => {
      const { data } = await apiClient.get('/api/benefits/summary');
      return data;
    },
  });

export const useCreateBenefit = (employeeId: number) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (dto: CreateBenefitRequest) => {
      const { data } = await apiClient.post<BenefitEnrollment>(`/api/employees/${employeeId}/benefits`, dto);
      return data;
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: [BENEFITS_KEY, employeeId] });
      qc.invalidateQueries({ queryKey: [SUMMARY_KEY] });
    },
  });
};

export const useUpdateBenefit = (employeeId: number) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async ({ benefitId, dto }: { benefitId: number; dto: UpdateBenefitRequest }) => {
      const { data } = await apiClient.put<BenefitEnrollment>(`/api/employees/${employeeId}/benefits/${benefitId}`, dto);
      return data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: [BENEFITS_KEY, employeeId] }),
  });
};

export const useDeleteBenefit = (employeeId: number) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (benefitId: number) => {
      await apiClient.delete(`/api/employees/${employeeId}/benefits/${benefitId}`);
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: [BENEFITS_KEY, employeeId] });
      qc.invalidateQueries({ queryKey: [SUMMARY_KEY] });
    },
  });
};
