using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Models;

namespace EmployeeBenefitsTracker.Api.Repositories;

public interface IEmployeeRepository
{
    Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, string? search, string? department, string? benefitStatus);
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee> CreateAsync(Employee employee);
    Task<Employee> UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);
}
