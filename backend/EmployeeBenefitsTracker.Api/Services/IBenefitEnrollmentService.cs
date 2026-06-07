using EmployeeBenefitsTracker.Api.DTOs;

namespace EmployeeBenefitsTracker.Api.Services;

public interface IBenefitEnrollmentService
{
    Task<List<BenefitEnrollmentResponseDto>> GetByEmployeeIdAsync(int employeeId);
    Task<BenefitEnrollmentResponseDto> CreateAsync(int employeeId, CreateBenefitEnrollmentDto dto);
    Task<BenefitEnrollmentResponseDto> UpdateAsync(int employeeId, int benefitId, UpdateBenefitEnrollmentDto dto);
    Task DeleteAsync(int employeeId, int benefitId);
    Task<BenefitSummaryDto> GetSummaryAsync();
}
