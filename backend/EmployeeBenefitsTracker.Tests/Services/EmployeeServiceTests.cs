using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Models;
using EmployeeBenefitsTracker.Api.Repositories;
using EmployeeBenefitsTracker.Api.Services;
using EmployeeBenefitsTracker.Api.Services.Exceptions;
using EmployeeBenefitsTracker.Tests.TestHelpers;
using Microsoft.Data.Sqlite;

namespace EmployeeBenefitsTracker.Tests.Services;

public class EmployeeServiceTests : IDisposable
{
    private readonly Api.Data.AppDbContext _context;
    private readonly SqliteConnection _connection;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        (_context, _connection) = TestDbContextFactory.Create();
        _service = new EmployeeService(new EmployeeRepository(_context));
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task CreateEmployee_ValidData_ReturnsEmployee()
    {
        var dto = new CreateEmployeeDto { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", Department = "Engineering", EmploymentStatus = EmploymentStatus.Active };
        var result = await _service.CreateAsync(dto);
        Assert.True(result.Id > 0);
        Assert.Equal("alice@example.com", result.Email);
        Assert.Equal("Alice", result.FirstName);
    }

    [Fact]
    public async Task CreateEmployee_DuplicateEmail_ThrowsArgumentException()
    {
        var dto = new CreateEmployeeDto { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", Department = "Engineering", EmploymentStatus = EmploymentStatus.Active };
        await _service.CreateAsync(dto);
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
        Assert.Contains("already in use", ex.Message);
    }

    [Fact]
    public async Task CreateEmployee_DuplicateEmailCaseInsensitive_ThrowsArgumentException()
    {
        await _service.CreateAsync(new CreateEmployeeDto { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", Department = "Engineering", EmploymentStatus = EmploymentStatus.Active });
        var dto2 = new CreateEmployeeDto { FirstName = "Bob", LastName = "Jones", Email = "ALICE@EXAMPLE.COM", Department = "HR", EmploymentStatus = EmploymentStatus.Active };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto2));
    }

    [Fact]
    public async Task CreateEmployee_MissingRequiredFields_ThrowsArgumentException()
    {
        var dto = new CreateEmployeeDto { FirstName = "", LastName = "Smith", Email = "a@b.com", Department = "HR", EmploymentStatus = EmploymentStatus.Active };
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task UpdateEmployee_ValidData_ReturnsUpdated()
    {
        var created = await _service.CreateAsync(new CreateEmployeeDto { FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", Department = "Engineering", EmploymentStatus = EmploymentStatus.Active });
        var result = await _service.UpdateAsync(created.Id, new UpdateEmployeeDto { FirstName = "Alicia", LastName = "Smith", Email = "alice@example.com", Department = "HR", EmploymentStatus = EmploymentStatus.Active });
        Assert.Equal("Alicia", result.FirstName);
        Assert.Equal("HR", result.Department);
    }

    [Fact]
    public async Task GetById_MissingEmployee_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(9999));
    }

    [Fact]
    public async Task UpdateEmployee_MissingEmployee_ThrowsNotFoundException()
    {
        var dto = new UpdateEmployeeDto { FirstName = "X", LastName = "Y", Email = "x@y.com", Department = "IT", EmploymentStatus = EmploymentStatus.Active };
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(9999, dto));
    }

    [Fact]
    public async Task DeleteEmployee_MissingEmployee_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(9999));
    }

    [Fact]
    public async Task DeleteEmployee_CascadesBenefitEnrollments()
    {
        var emp = await _service.CreateAsync(new CreateEmployeeDto { FirstName = "Bob", LastName = "Jones", Email = "bob@example.com", Department = "IT", EmploymentStatus = EmploymentStatus.Active });
        _context.BenefitEnrollments.Add(new BenefitEnrollment { EmployeeId = emp.Id, BenefitType = BenefitType.Health, EnrollmentStatus = EnrollmentStatus.Enrolled, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });
        await _context.SaveChangesAsync();
        await _service.DeleteAsync(emp.Id);
        Assert.Equal(0, _context.BenefitEnrollments.Count());
    }
}
