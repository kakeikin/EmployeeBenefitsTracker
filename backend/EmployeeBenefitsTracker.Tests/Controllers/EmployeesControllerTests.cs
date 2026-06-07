using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Tests.TestHelpers;

namespace EmployeeBenefitsTracker.Tests.Controllers;

public class EmployeesControllerTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public EmployeesControllerTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose() { _client.Dispose(); _factory.Dispose(); }

    private Task<HttpResponseMessage> CreateEmployee(string firstName = "Alice", string email = "alice@test.com") =>
        _client.PostAsJsonAsync("/api/employees", new { firstName, lastName = "Smith", email, department = "Engineering", employmentStatus = "Active" });

    [Fact]
    public async Task GetAll_ReturnsOkWithPagedResult()
    {
        var response = await _client.GetAsync("/api/employees");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = JsonSerializer.Deserialize<PagedResult<EmployeeResponseDto>>(
            await response.Content.ReadAsStringAsync(), JsonOpts);
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
    }

    [Fact]
    public async Task GetAll_WithFilters_ReturnsFilteredResult()
    {
        await CreateEmployee("Alice", "alice.eng@test.com");
        await _client.PostAsJsonAsync("/api/employees", new { firstName = "Bob", lastName = "Jones", email = "bob.hr@test.com", department = "HR", employmentStatus = "Active" });

        var response = await _client.GetAsync("/api/employees?department=Engineering");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = JsonSerializer.Deserialize<PagedResult<EmployeeResponseDto>>(
            await response.Content.ReadAsStringAsync(), JsonOpts);
        Assert.NotNull(result);
        Assert.All(result.Items, e => Assert.Equal("Engineering", e.Department));
    }

    [Fact]
    public async Task CreateEmployee_ReturnsCreated()
    {
        var response = await CreateEmployee();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var emp = JsonSerializer.Deserialize<EmployeeResponseDto>(
            await response.Content.ReadAsStringAsync(), JsonOpts);
        Assert.NotNull(emp);
        Assert.True(emp.Id > 0);
    }

    [Fact]
    public async Task UpdateEmployee_MissingEmployee_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync("/api/employees/9999",
            new { firstName = "X", lastName = "Y", email = "x@y.com", department = "IT", employmentStatus = "Active" });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteEmployee_ReturnsNoContent()
    {
        var createResponse = await CreateEmployee("DeleteMe", "delete@test.com");
        var created = JsonSerializer.Deserialize<EmployeeResponseDto>(
            await createResponse.Content.ReadAsStringAsync(), JsonOpts)!;

        var deleteResponse = await _client.DeleteAsync($"/api/employees/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
