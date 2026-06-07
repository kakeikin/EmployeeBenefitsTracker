using EmployeeBenefitsTracker.Api.DTOs;

namespace EmployeeBenefitsTracker.Api.Services;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeResponseDto>> GetPagedAsync(int page, int pageSize, string? search, string? department, string? benefitStatus);
    Task<EmployeeResponseDto> GetByIdAsync(int id);
    Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto);
    Task<EmployeeResponseDto> UpdateAsync(int id, UpdateEmployeeDto dto);
    Task DeleteAsync(int id);
}
