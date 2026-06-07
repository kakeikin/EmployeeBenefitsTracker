using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Models;
using EmployeeBenefitsTracker.Api.Repositories;
using EmployeeBenefitsTracker.Api.Services.Exceptions;

namespace EmployeeBenefitsTracker.Api.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;

    public EmployeeService(IEmployeeRepository repo) => _repo = repo;

    public async Task<PagedResult<EmployeeResponseDto>> GetPagedAsync(int page, int pageSize, string? search, string? department, string? benefitStatus)
    {
        var result = await _repo.GetPagedAsync(page, pageSize, search, department, benefitStatus);
        return new PagedResult<EmployeeResponseDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<EmployeeResponseDto> GetByIdAsync(int id)
    {
        var employee = await _repo.GetByIdAsync(id);
        if (employee == null) throw new NotFoundException($"Employee {id} not found.");
        return MapToDto(employee);
    }

    public async Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto)
    {
        ValidateDto(dto.FirstName, dto.LastName, dto.Email, dto.Department);
        var existing = await _repo.GetByEmailAsync(dto.Email.Trim().ToLower());
        if (existing != null) throw new ArgumentException($"Email '{dto.Email}' is already in use.");
        var employee = new Employee
        {
            FirstName = dto.FirstName.Trim(),
            LastName = dto.LastName.Trim(),
            Email = dto.Email.Trim().ToLower(),
            Department = dto.Department.Trim(),
            EmploymentStatus = dto.EmploymentStatus,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return MapToDto(await _repo.CreateAsync(employee));
    }

    public async Task<EmployeeResponseDto> UpdateAsync(int id, UpdateEmployeeDto dto)
    {
        var employee = await _repo.GetByIdAsync(id) ?? throw new NotFoundException($"Employee {id} not found.");
        ValidateDto(dto.FirstName, dto.LastName, dto.Email, dto.Department);
        var existing = await _repo.GetByEmailAsync(dto.Email.Trim().ToLower());
        if (existing != null && existing.Id != id)
            throw new ArgumentException($"Email '{dto.Email}' is already in use.");
        employee.FirstName = dto.FirstName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.Email = dto.Email.Trim().ToLower();
        employee.Department = dto.Department.Trim();
        employee.EmploymentStatus = dto.EmploymentStatus;
        employee.UpdatedAt = DateTime.UtcNow;
        return MapToDto(await _repo.UpdateAsync(employee));
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _repo.GetByIdAsync(id) ?? throw new NotFoundException($"Employee {id} not found.");
        await _repo.DeleteAsync(employee);
    }

    private static void ValidateDto(string firstName, string lastName, string email, string department)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(department)) throw new ArgumentException("Department is required.");
        try { _ = new System.Net.Mail.MailAddress(email); }
        catch { throw new ArgumentException("Email format is invalid."); }
    }

    private static EmployeeResponseDto MapToDto(Employee e) => new()
    {
        Id = e.Id, FirstName = e.FirstName, LastName = e.LastName,
        Email = e.Email, Department = e.Department,
        EmploymentStatus = e.EmploymentStatus.ToString(),
        CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt
    };
}
