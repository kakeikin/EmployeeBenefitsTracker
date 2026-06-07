export const formatDate = (iso: string | null | undefined): string => {
  if (!iso) return '—';
  return new Date(iso).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
};

export const formatPercent = (rate: number): string => `${(rate * 100).toFixed(1)}%`;

export const DEPARTMENTS = ['Engineering', 'HR', 'Finance', 'Marketing', 'Sales', 'Operations', 'Legal', 'IT'];
export const BENEFIT_TYPES = ['Health', 'Dental', 'Vision', 'Retirement', 'Life'] as const;
export const ENROLLMENT_STATUSES = ['NotEnrolled', 'Pending', 'Enrolled', 'Waived'] as const;
export const EMPLOYMENT_STATUSES = ['Active', 'Inactive'] as const;
