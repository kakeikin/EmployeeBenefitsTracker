using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Models;
using EmployeeBenefitsTracker.Api.Repositories;
using EmployeeBenefitsTracker.Api.Services;
using EmployeeBenefitsTracker.Api.Services.Exceptions;
using EmployeeBenefitsTracker.Tests.TestHelpers;
using Microsoft.Data.Sqlite;

namespace EmployeeBenefitsTracker.Tests.Services;

public class BenefitEnrollmentServiceTests : IDisposable
{
    private readonly Api.Data.AppDbContext _context;
    private readonly SqliteConnection _connection;
    private readonly BenefitEnrollmentService _benefitService;
    private readonly EmployeeService _employeeService;

    public BenefitEnrollmentServiceTests()
    {
        (_context, _connection) = TestDbContextFactory.Create();
        var empRepo = new EmployeeRepository(_context);
        var benefitRepo = new BenefitEnrollmentRepository(_context);
        _employeeService = new EmployeeService(empRepo);
        _benefitService = new BenefitEnrollmentService(benefitRepo, empRepo);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    private Task<EmployeeResponseDto> CreateTestEmployee(string email = "test@example.com") =>
        _employeeService.CreateAsync(new CreateEmployeeDto { FirstName = "Test", LastName = "User", Email = email, Department = "IT", EmploymentStatus = EmploymentStatus.Active });

    [Fact]
    public async Task AddBenefit_ValidData_ReturnsBenefitEnrollment()
    {
        var emp = await CreateTestEmployee();
        var result = await _benefitService.CreateAsync(emp.Id, new CreateBenefitEnrollmentDto { BenefitType = BenefitType.Health, EnrollmentStatus = EnrollmentStatus.Enrolled });
        Assert.Equal("Health", result.BenefitType);
        Assert.Equal("Enrolled", result.EnrollmentStatus);
        Assert.Equal(emp.Id, result.EmployeeId);
    }

    [Fact]
    public async Task UpdateBenefitStatus_ValidData_ReturnsUpdated()
    {
        var emp = await CreateTestEmployee();
        var created = await _benefitService.CreateAsync(emp.Id, new CreateBenefitEnrollmentDto { BenefitType = BenefitType.Dental, EnrollmentStatus = EnrollmentStatus.Pending });
        var result = await _benefitService.UpdateAsync(emp.Id, created.Id, new UpdateBenefitEnrollmentDto { EnrollmentStatus = EnrollmentStatus.Enrolled });
        Assert.Equal("Enrolled", result.EnrollmentStatus);
    }

    [Fact]
    public async Task AddBenefit_MissingEmployee_ThrowsNotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _benefitService.CreateAsync(9999, new CreateBenefitEnrollmentDto { BenefitType = BenefitType.Health, EnrollmentStatus = EnrollmentStatus.Pending }));
    }

    [Fact]
    public async Task AddBenefit_DuplicateBenefitType_ThrowsArgumentException()
    {
        var emp = await CreateTestEmployee();
        await _benefitService.CreateAsync(emp.Id, new CreateBenefitEnrollmentDto { BenefitType = BenefitType.Health, EnrollmentStatus = EnrollmentStatus.Enrolled });
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _benefitService.CreateAsync(emp.Id, new CreateBenefitEnrollmentDto { BenefitType = BenefitType.Health, EnrollmentStatus = EnrollmentStatus.Pending }));
        Assert.Contains("Health", ex.Message);
    }

    [Fact]
    public async Task UpdateBenefit_BenefitBelongsToOtherEmployee_ThrowsNotFoundException()
    {
        var emp1 = await CreateTestEmployee("emp1@example.com");
        var emp2 = await CreateTestEmployee("emp2@example.com");
        var benefit = await _benefitService.CreateAsync(emp1.Id, new CreateBenefitEnrollmentDto { BenefitType = BenefitType.Vision, EnrollmentStatus = EnrollmentStatus.Enrolled });
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _benefitService.UpdateAsync(emp2.Id, benefit.Id, new UpdateBenefitEnrollmentDto { EnrollmentStatus = EnrollmentStatus.Waived }));
    }

    [Fact]
    public async Task GetSummary_ReturnsCorrectCounts()
    {
        var emp1 = await CreateTestEmployee("emp1@example.com");
        var emp2 = await CreateTestEmployee("emp2@example.com");

        // emp1 gets Health (Enrolled), Dental (Pending)
        await _benefitService.CreateAsync(emp1.Id, new CreateBenefitEnrollmentDto { BenefitType = BenefitType.Health, EnrollmentStatus = EnrollmentStatus.Enrolled });
        await _benefitService.CreateAsync(emp1.Id, new CreateBenefitEnrollmentDto { BenefitType = BenefitType.Dental, EnrollmentStatus = EnrollmentStatus.Pending });

        // emp2 gets Health (Enrolled)
        await _benefitService.CreateAsync(emp2.Id, new CreateBenefitEnrollmentDto { BenefitType = BenefitType.Health, EnrollmentStatus = EnrollmentStatus.Enrolled });

        var summary = await _benefitService.GetSummaryAsync();

        Assert.Equal(2, summary.TotalEmployees);
        Assert.Equal(2, summary.ActiveEmployees);
        Assert.Equal(1, summary.PendingEnrollments);
        Assert.Equal(2, summary.EnrolledByBenefitType["Health"]);
        Assert.Equal(0, summary.EnrolledByBenefitType["Dental"]); // Dental is Pending, not Enrolled
        // Overall rate: 2 enrolled / (2 employees * 5 benefit types) = 0.2
        Assert.Equal(0.2, summary.OverallEnrollmentRate);
    }
}
