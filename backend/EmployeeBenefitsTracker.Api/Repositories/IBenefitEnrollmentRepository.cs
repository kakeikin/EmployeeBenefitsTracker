using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Models;

namespace EmployeeBenefitsTracker.Api.Repositories;

public interface IBenefitEnrollmentRepository
{
    Task<List<BenefitEnrollment>> GetByEmployeeIdAsync(int employeeId);
    Task<BenefitEnrollment?> GetByIdAsync(int id);
    Task<BenefitEnrollment?> GetByEmployeeAndTypeAsync(int employeeId, BenefitType benefitType);
    Task<BenefitEnrollment> CreateAsync(BenefitEnrollment enrollment);
    Task<BenefitEnrollment> UpdateAsync(BenefitEnrollment enrollment);
    Task DeleteAsync(BenefitEnrollment enrollment);
    Task<BenefitSummaryDto> GetSummaryAsync();
}
