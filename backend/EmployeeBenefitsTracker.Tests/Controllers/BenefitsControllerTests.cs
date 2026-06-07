using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Tests.TestHelpers;

namespace EmployeeBenefitsTracker.Tests.Controllers;

public class BenefitsControllerTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public BenefitsControllerTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose() { _client.Dispose(); _factory.Dispose(); }

    private async Task<EmployeeResponseDto> CreateEmployee(string email = "emp@test.com")
    {
        var response = await _client.PostAsJsonAsync("/api/employees",
            new { firstName = "Test", lastName = "User", email, department = "IT", employmentStatus = "Active" });
        return JsonSerializer.Deserialize<EmployeeResponseDto>(
            await response.Content.ReadAsStringAsync(), JsonOpts)!;
    }

    [Fact]
    public async Task GetSummary_ReturnsOkWithCounts()
    {
        var response = await _client.GetAsync("/api/benefits/summary");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var summary = JsonSerializer.Deserialize<BenefitSummaryDto>(
            await response.Content.ReadAsStringAsync(), JsonOpts);
        Assert.NotNull(summary);
        Assert.NotNull(summary.EnrolledByBenefitType);
    }

    [Fact]
    public async Task CreateBenefit_MissingEmployee_ReturnsNotFound()
    {
        var response = await _client.PostAsJsonAsync("/api/employees/9999/benefits",
            new { benefitType = "Health", enrollmentStatus = "Pending" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateBenefit_ValidEmployee_ReturnsCreated()
    {
        var emp = await CreateEmployee();
        var response = await _client.PostAsJsonAsync($"/api/employees/{emp.Id}/benefits",
            new { benefitType = "Health", enrollmentStatus = "Enrolled" });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateBenefit_BenefitBelongsToOtherEmployee_ReturnsNotFound()
    {
        var emp1 = await CreateEmployee("emp1b@test.com");
        var emp2 = await CreateEmployee("emp2b@test.com");
        var benefitResponse = await _client.PostAsJsonAsync($"/api/employees/{emp1.Id}/benefits",
            new { benefitType = "Dental", enrollmentStatus = "Pending" });
        var benefit = JsonSerializer.Deserialize<BenefitEnrollmentResponseDto>(
            await benefitResponse.Content.ReadAsStringAsync(), JsonOpts)!;

        var response = await _client.PutAsJsonAsync($"/api/employees/{emp2.Id}/benefits/{benefit.Id}",
            new { enrollmentStatus = "Waived" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
