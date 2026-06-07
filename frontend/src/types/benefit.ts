export type BenefitType = 'Health' | 'Dental' | 'Vision' | 'Retirement' | 'Life';
export type EnrollmentStatus = 'NotEnrolled' | 'Pending' | 'Enrolled' | 'Waived';

export interface BenefitEnrollment {
  id: number;
  employeeId: number;
  benefitType: BenefitType;
  enrollmentStatus: EnrollmentStatus;
  effectiveDate: string | null;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBenefitRequest {
  benefitType: BenefitType;
  enrollmentStatus: EnrollmentStatus;
  effectiveDate?: string;
  notes?: string;
}

export interface UpdateBenefitRequest {
  enrollmentStatus: EnrollmentStatus;
  effectiveDate?: string;
  notes?: string;
}

export interface BenefitSummary {
  totalEmployees: number;
  activeEmployees: number;
  pendingEnrollments: number;
  enrolledByBenefitType: Record<string, number>;
  overallEnrollmentRate: number;
}
