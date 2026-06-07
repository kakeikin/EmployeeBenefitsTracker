using EmployeeBenefitsTracker.Api.Data;
using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeBenefitsTracker.Api.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context) => _context = context;

    public async Task<PagedResult<Employee>> GetPagedAsync(int page, int pageSize, string? search, string? department, string? benefitStatus)
    {
        var query = _context.Employees.Include(e => e.BenefitEnrollments).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(e =>
                e.FirstName.ToLower().Contains(lower) ||
                e.LastName.ToLower().Contains(lower) ||
                e.Email.ToLower().Contains(lower));
        }

        if (!string.IsNullOrWhiteSpace(department))
            query = query.Where(e => e.Department == department);

        if (!string.IsNullOrWhiteSpace(benefitStatus) && Enum.TryParse<EnrollmentStatus>(benefitStatus, out var status))
            query = query.Where(e => e.BenefitEnrollments.Any(b => b.EnrollmentStatus == status));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync();

        return new PagedResult<Employee> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }

    public async Task<Employee?> GetByIdAsync(int id) =>
        await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Employee?> GetByEmailAsync(string email) =>
        await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);

    public async Task<Employee> CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}
