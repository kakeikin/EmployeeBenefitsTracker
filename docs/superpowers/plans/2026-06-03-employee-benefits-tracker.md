# Employee Benefits Tracker Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a full-stack HR tool with ASP.NET Core REST APIs, React TypeScript dashboard, SQLite persistence, xUnit tests, and GitHub Actions CI.

**Architecture:** Layered backend (Controllers → Services → Repositories → EF Core/SQLite) with a Vite React TypeScript frontend using React Query for server state. Two separate .NET projects (API + Tests) in one solution.

**Tech Stack:** C# / ASP.NET Core 8, Entity Framework Core 8, SQLite, xUnit, WebApplicationFactory, React 18, TypeScript, Vite, Tailwind CSS v4, React Query v5, Axios, React Router v6, GitHub Actions.

---

## File Map

```
EmployeeBenefitsTracker/
├── backend/
│   ├── EmployeeBenefitsTracker.sln
│   ├── EmployeeBenefitsTracker.Api/
│   │   ├── EmployeeBenefitsTracker.Api.csproj
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── Properties/launchSettings.json
│   │   ├── Models/
│   │   │   ├── Employee.cs
│   │   │   ├── BenefitEnrollment.cs
│   │   │   └── Enums.cs
│   │   ├── DTOs/
│   │   │   ├── EmployeeDtos.cs
│   │   │   ├── BenefitDtos.cs
│   │   │   └── PagedResult.cs
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   └── DatabaseSeeder.cs
│   │   ├── Repositories/
│   │   │   ├── IEmployeeRepository.cs
│   │   │   ├── EmployeeRepository.cs
│   │   │   ├── IBenefitEnrollmentRepository.cs
│   │   │   └── BenefitEnrollmentRepository.cs
│   │   ├── Services/
│   │   │   ├── Exceptions/NotFoundException.cs
│   │   │   ├── IEmployeeService.cs
│   │   │   ├── EmployeeService.cs
│   │   │   ├── IBenefitEnrollmentService.cs
│   │   │   └── BenefitEnrollmentService.cs
│   │   └── Controllers/
│   │       ├── EmployeesController.cs
│   │       └── BenefitsController.cs
│   └── EmployeeBenefitsTracker.Tests/
│       ├── EmployeeBenefitsTracker.Tests.csproj
│       ├── TestHelpers/
│       │   ├── TestDbContextFactory.cs
│       │   └── CustomWebApplicationFactory.cs
│       ├── Services/
│       │   ├── EmployeeServiceTests.cs
│       │   └── BenefitEnrollmentServiceTests.cs
│       └── Controllers/
│           ├── EmployeesControllerTests.cs
│           └── BenefitsControllerTests.cs
├── frontend/
│   ├── package.json
│   ├── tsconfig.json
│   ├── tsconfig.app.json
│   ├── vite.config.ts
│   ├── index.html
│   ├── .env
│   └── src/
│       ├── main.tsx
│       ├── App.tsx
│       ├── index.css
│       ├── types/
│       │   ├── employee.ts
│       │   └── benefit.ts
│       ├── api/
│       │   ├── axios.ts
│       │   ├── employees.ts
│       │   └── benefits.ts
│       ├── components/
│       │   ├── Badge.tsx
│       │   ├── SummaryCard.tsx
│       │   ├── Pagination.tsx
│       │   ├── Modal.tsx
│       │   └── FormField.tsx
│       ├── pages/
│       │   ├── EmployeeList.tsx
│       │   ├── EmployeeForm.tsx
│       │   └── EmployeeDetail.tsx
│       └── utils/
│           └── formatters.ts
└── .github/
    └── workflows/
        └── ci.yml
```

---

## Task 1: Scaffold .NET Solution

**Files:**
- Create: `backend/EmployeeBenefitsTracker.sln`
- Create: `backend/EmployeeBenefitsTracker.Api/EmployeeBenefitsTracker.Api.csproj`
- Create: `backend/EmployeeBenefitsTracker.Tests/EmployeeBenefitsTracker.Tests.csproj`

- [ ] **Step 1: Create directory and scaffold solution**

```bash
mkdir -p EmployeeBenefitsTracker/backend
cd EmployeeBenefitsTracker/backend
dotnet new sln -n EmployeeBenefitsTracker
dotnet new webapi -n EmployeeBenefitsTracker.Api --use-controllers --no-https
dotnet new xunit -n EmployeeBenefitsTracker.Tests
dotnet sln add EmployeeBenefitsTracker.Api/EmployeeBenefitsTracker.Api.csproj
dotnet sln add EmployeeBenefitsTracker.Tests/EmployeeBenefitsTracker.Tests.csproj
dotnet add EmployeeBenefitsTracker.Tests/EmployeeBenefitsTracker.Tests.csproj reference EmployeeBenefitsTracker.Api/EmployeeBenefitsTracker.Api.csproj
```

- [ ] **Step 2: Add NuGet packages to API project**

```bash
cd EmployeeBenefitsTracker.Api
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
cd ..
```

- [ ] **Step 3: Add NuGet packages to Tests project**

```bash
cd EmployeeBenefitsTracker.Tests
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.Data.Sqlite
cd ..
```

- [ ] **Step 4: Set launch URL to port 5000**

Replace `EmployeeBenefitsTracker.Api/Properties/launchSettings.json` content:

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

- [ ] **Step 5: Delete generated boilerplate from webapi template**

Delete `EmployeeBenefitsTracker.Api/Controllers/WeatherForecastController.cs` and `EmployeeBenefitsTracker.Api/WeatherForecast.cs` if they exist.

- [ ] **Step 6: Verify solution builds**

```bash
cd EmployeeBenefitsTracker/backend
dotnet build
```

Expected: Build succeeded, 0 errors.

---

## Task 2: Models and Enums

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Api/Models/Enums.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/Models/Employee.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/Models/BenefitEnrollment.cs`

- [ ] **Step 1: Create enums**

```csharp
// Models/Enums.cs
namespace EmployeeBenefitsTracker.Api.Models;

public enum EmploymentStatus { Active, Inactive }
public enum BenefitType { Health, Dental, Vision, Retirement, Life }
public enum EnrollmentStatus { NotEnrolled, Pending, Enrolled, Waived }
```

- [ ] **Step 2: Create Employee entity**

```csharp
// Models/Employee.cs
namespace EmployeeBenefitsTracker.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public EmploymentStatus EmploymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<BenefitEnrollment> BenefitEnrollments { get; set; } = new();
}
```

- [ ] **Step 3: Create BenefitEnrollment entity**

```csharp
// Models/BenefitEnrollment.cs
namespace EmployeeBenefitsTracker.Api.Models;

public class BenefitEnrollment
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public BenefitType BenefitType { get; set; }
    public EnrollmentStatus EnrollmentStatus { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Employee Employee { get; set; } = null!;
}
```

---

## Task 3: DTOs

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Api/DTOs/PagedResult.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/DTOs/EmployeeDtos.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/DTOs/BenefitDtos.cs`

- [ ] **Step 1: Create PagedResult**

```csharp
// DTOs/PagedResult.cs
namespace EmployeeBenefitsTracker.Api.DTOs;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
```

- [ ] **Step 2: Create employee DTOs**

```csharp
// DTOs/EmployeeDtos.cs
using EmployeeBenefitsTracker.Api.Models;

namespace EmployeeBenefitsTracker.Api.DTOs;

public class CreateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.Active;
}

public class UpdateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public EmploymentStatus EmploymentStatus { get; set; } = EmploymentStatus.Active;
}

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string EmploymentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

- [ ] **Step 3: Create benefit DTOs**

```csharp
// DTOs/BenefitDtos.cs
using EmployeeBenefitsTracker.Api.Models;

namespace EmployeeBenefitsTracker.Api.DTOs;

public class CreateBenefitEnrollmentDto
{
    public BenefitType BenefitType { get; set; }
    public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.Pending;
    public DateTime? EffectiveDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateBenefitEnrollmentDto
{
    public EnrollmentStatus EnrollmentStatus { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string? Notes { get; set; }
}

public class BenefitEnrollmentResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string BenefitType { get; set; } = string.Empty;
    public string EnrollmentStatus { get; set; } = string.Empty;
    public DateTime? EffectiveDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class BenefitSummaryDto
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int PendingEnrollments { get; set; }
    public Dictionary<string, int> EnrolledByBenefitType { get; set; } = new();
    public double OverallEnrollmentRate { get; set; }
}
```

---

## Task 4: AppDbContext

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Api/Data/AppDbContext.cs`
- Modify: `backend/EmployeeBenefitsTracker.Api/appsettings.json`

- [ ] **Step 1: Create AppDbContext**

```csharp
// Data/AppDbContext.cs
using EmployeeBenefitsTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeBenefitsTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<BenefitEnrollment> BenefitEnrollments => Set<BenefitEnrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .Property(e => e.EmploymentStatus)
            .HasConversion<string>();

        modelBuilder.Entity<BenefitEnrollment>()
            .Property(b => b.BenefitType)
            .HasConversion<string>();

        modelBuilder.Entity<BenefitEnrollment>()
            .Property(b => b.EnrollmentStatus)
            .HasConversion<string>();

        modelBuilder.Entity<BenefitEnrollment>()
            .HasOne(b => b.Employee)
            .WithMany(e => e.BenefitEnrollments)
            .HasForeignKey(b => b.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BenefitEnrollment>()
            .HasIndex(b => new { b.EmployeeId, b.BenefitType })
            .IsUnique();
    }
}
```

- [ ] **Step 2: Update appsettings.json**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AllowedOrigins": "http://localhost:5173"
}
```

---

## Task 5: NotFoundException

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Api/Services/Exceptions/NotFoundException.cs`

- [ ] **Step 1: Create exception**

```csharp
// Services/Exceptions/NotFoundException.cs
namespace EmployeeBenefitsTracker.Api.Services.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
```

---

## Task 6: Repositories

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Api/Repositories/IEmployeeRepository.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/Repositories/EmployeeRepository.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/Repositories/IBenefitEnrollmentRepository.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/Repositories/BenefitEnrollmentRepository.cs`

- [ ] **Step 1: Create IEmployeeRepository**

```csharp
// Repositories/IEmployeeRepository.cs
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
```

- [ ] **Step 2: Create EmployeeRepository**

```csharp
// Repositories/EmployeeRepository.cs
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
        var query = _context.Employees.Include(e => e.BenefitEnrollments).AsQueryable();

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
        await _context.Employees.Include(e => e.BenefitEnrollments).FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Employee?> GetByEmailAsync(string email) =>
        await _context.Employees.FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());

    public async Task<Employee> CreateAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}
```

- [ ] **Step 3: Create IBenefitEnrollmentRepository**

```csharp
// Repositories/IBenefitEnrollmentRepository.cs
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
```

- [ ] **Step 4: Create BenefitEnrollmentRepository**

```csharp
// Repositories/BenefitEnrollmentRepository.cs
using EmployeeBenefitsTracker.Api.Data;
using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeBenefitsTracker.Api.Repositories;

public class BenefitEnrollmentRepository : IBenefitEnrollmentRepository
{
    private readonly AppDbContext _context;

    public BenefitEnrollmentRepository(AppDbContext context) => _context = context;

    public async Task<List<BenefitEnrollment>> GetByEmployeeIdAsync(int employeeId) =>
        await _context.BenefitEnrollments.Where(b => b.EmployeeId == employeeId).ToListAsync();

    public async Task<BenefitEnrollment?> GetByIdAsync(int id) =>
        await _context.BenefitEnrollments.FindAsync(id);

    public async Task<BenefitEnrollment?> GetByEmployeeAndTypeAsync(int employeeId, BenefitType benefitType) =>
        await _context.BenefitEnrollments
            .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.BenefitType == benefitType);

    public async Task<BenefitEnrollment> CreateAsync(BenefitEnrollment enrollment)
    {
        _context.BenefitEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task<BenefitEnrollment> UpdateAsync(BenefitEnrollment enrollment)
    {
        _context.BenefitEnrollments.Update(enrollment);
        await _context.SaveChangesAsync();
        return enrollment;
    }

    public async Task DeleteAsync(BenefitEnrollment enrollment)
    {
        _context.BenefitEnrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task<BenefitSummaryDto> GetSummaryAsync()
    {
        var totalEmployees = await _context.Employees.CountAsync();
        var activeEmployees = await _context.Employees.CountAsync(e => e.EmploymentStatus == EmploymentStatus.Active);
        var pendingEnrollments = await _context.BenefitEnrollments.CountAsync(b => b.EnrollmentStatus == EnrollmentStatus.Pending);

        var enrolledByType = new Dictionary<string, int>();
        foreach (var type in Enum.GetValues<BenefitType>())
        {
            enrolledByType[type.ToString()] = await _context.BenefitEnrollments
                .CountAsync(b => b.BenefitType == type && b.EnrollmentStatus == EnrollmentStatus.Enrolled);
        }

        var totalEnrolled = enrolledByType.Values.Sum();
        var rate = totalEmployees > 0 ? Math.Round((double)totalEnrolled / (totalEmployees * 5), 4) : 0;

        return new BenefitSummaryDto
        {
            TotalEmployees = totalEmployees,
            ActiveEmployees = activeEmployees,
            PendingEnrollments = pendingEnrollments,
            EnrolledByBenefitType = enrolledByType,
            OverallEnrollmentRate = rate
        };
    }
}
```

---

## Task 7: Test Project Setup

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Tests/TestHelpers/TestDbContextFactory.cs`
- Create: `backend/EmployeeBenefitsTracker.Tests/TestHelpers/CustomWebApplicationFactory.cs`

- [ ] **Step 1: Create TestDbContextFactory**

```csharp
// TestHelpers/TestDbContextFactory.cs
using EmployeeBenefitsTracker.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EmployeeBenefitsTracker.Tests.TestHelpers;

public static class TestDbContextFactory
{
    public static (AppDbContext context, SqliteConnection connection) Create()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return (context, connection);
    }
}
```

- [ ] **Step 2: Create CustomWebApplicationFactory**

```csharp
// TestHelpers/CustomWebApplicationFactory.cs
using EmployeeBenefitsTracker.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeBenefitsTracker.Tests.TestHelpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose();
    }
}
```

- [ ] **Step 3: Add `public partial class Program { }` to the end of Program.cs**

This makes the `Program` class visible to the test project. Add at the bottom of `EmployeeBenefitsTracker.Api/Program.cs` (even before the full Program.cs is written, add a placeholder file that at minimum has this line plus minimal startup so it compiles):

```csharp
public partial class Program { }
```

---

## Task 8: Employee Service Tests (Write Failing Tests First)

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Tests/Services/EmployeeServiceTests.cs`

- [ ] **Step 1: Write all employee service tests**

```csharp
// Services/EmployeeServiceTests.cs
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
```

- [ ] **Step 2: Run tests — expect compile failure (service not yet implemented)**

```bash
cd EmployeeBenefitsTracker/backend
dotnet test EmployeeBenefitsTracker.Tests --no-build 2>&1 | head -30
```

Expected: Build errors about missing `EmployeeService` class.

---

## Task 9: Employee Service Implementation

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Api/Services/IEmployeeService.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/Services/EmployeeService.cs`

- [ ] **Step 1: Create IEmployeeService**

```csharp
// Services/IEmployeeService.cs
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
```

- [ ] **Step 2: Create EmployeeService**

```csharp
// Services/EmployeeService.cs
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
        var existing = await _repo.GetByEmailAsync(dto.Email);
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
        var existing = await _repo.GetByEmailAsync(dto.Email);
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
```

- [ ] **Step 3: Run employee service tests — all should pass**

```bash
cd EmployeeBenefitsTracker/backend
dotnet test EmployeeBenefitsTracker.Tests --filter "EmployeeServiceTests" -v normal
```

Expected: 9 tests pass.

---

## Task 10: Benefit Service Tests and Implementation

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Tests/Services/BenefitEnrollmentServiceTests.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/Services/IBenefitEnrollmentService.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/Services/BenefitEnrollmentService.cs`

- [ ] **Step 1: Write benefit service tests**

```csharp
// Services/BenefitEnrollmentServiceTests.cs
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
}
```

- [ ] **Step 2: Run tests — expect compile failure**

```bash
dotnet build EmployeeBenefitsTracker.Tests 2>&1 | head -20
```

Expected: Build error about missing `BenefitEnrollmentService`.

- [ ] **Step 3: Create IBenefitEnrollmentService**

```csharp
// Services/IBenefitEnrollmentService.cs
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
```

- [ ] **Step 4: Create BenefitEnrollmentService**

```csharp
// Services/BenefitEnrollmentService.cs
using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Models;
using EmployeeBenefitsTracker.Api.Repositories;
using EmployeeBenefitsTracker.Api.Services.Exceptions;

namespace EmployeeBenefitsTracker.Api.Services;

public class BenefitEnrollmentService : IBenefitEnrollmentService
{
    private readonly IBenefitEnrollmentRepository _repo;
    private readonly IEmployeeRepository _employeeRepo;

    public BenefitEnrollmentService(IBenefitEnrollmentRepository repo, IEmployeeRepository employeeRepo)
    {
        _repo = repo;
        _employeeRepo = employeeRepo;
    }

    public async Task<List<BenefitEnrollmentResponseDto>> GetByEmployeeIdAsync(int employeeId)
    {
        if (await _employeeRepo.GetByIdAsync(employeeId) == null)
            throw new NotFoundException($"Employee {employeeId} not found.");
        return (await _repo.GetByEmployeeIdAsync(employeeId)).Select(MapToDto).ToList();
    }

    public async Task<BenefitEnrollmentResponseDto> CreateAsync(int employeeId, CreateBenefitEnrollmentDto dto)
    {
        if (await _employeeRepo.GetByIdAsync(employeeId) == null)
            throw new NotFoundException($"Employee {employeeId} not found.");
        var existing = await _repo.GetByEmployeeAndTypeAsync(employeeId, dto.BenefitType);
        if (existing != null)
            throw new ArgumentException($"Employee already has a {dto.BenefitType} enrollment.");
        var enrollment = new BenefitEnrollment
        {
            EmployeeId = employeeId,
            BenefitType = dto.BenefitType,
            EnrollmentStatus = dto.EnrollmentStatus,
            EffectiveDate = dto.EffectiveDate,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return MapToDto(await _repo.CreateAsync(enrollment));
    }

    public async Task<BenefitEnrollmentResponseDto> UpdateAsync(int employeeId, int benefitId, UpdateBenefitEnrollmentDto dto)
    {
        if (await _employeeRepo.GetByIdAsync(employeeId) == null)
            throw new NotFoundException($"Employee {employeeId} not found.");
        var enrollment = await _repo.GetByIdAsync(benefitId);
        if (enrollment == null || enrollment.EmployeeId != employeeId)
            throw new NotFoundException($"Benefit enrollment {benefitId} not found for employee {employeeId}.");
        enrollment.EnrollmentStatus = dto.EnrollmentStatus;
        enrollment.EffectiveDate = dto.EffectiveDate;
        enrollment.Notes = dto.Notes;
        enrollment.UpdatedAt = DateTime.UtcNow;
        return MapToDto(await _repo.UpdateAsync(enrollment));
    }

    public async Task DeleteAsync(int employeeId, int benefitId)
    {
        if (await _employeeRepo.GetByIdAsync(employeeId) == null)
            throw new NotFoundException($"Employee {employeeId} not found.");
        var enrollment = await _repo.GetByIdAsync(benefitId);
        if (enrollment == null || enrollment.EmployeeId != employeeId)
            throw new NotFoundException($"Benefit enrollment {benefitId} not found for employee {employeeId}.");
        await _repo.DeleteAsync(enrollment);
    }

    public Task<BenefitSummaryDto> GetSummaryAsync() => _repo.GetSummaryAsync();

    private static BenefitEnrollmentResponseDto MapToDto(BenefitEnrollment b) => new()
    {
        Id = b.Id, EmployeeId = b.EmployeeId,
        BenefitType = b.BenefitType.ToString(),
        EnrollmentStatus = b.EnrollmentStatus.ToString(),
        EffectiveDate = b.EffectiveDate, Notes = b.Notes,
        CreatedAt = b.CreatedAt, UpdatedAt = b.UpdatedAt
    };
}
```

- [ ] **Step 5: Run all service tests — all should pass**

```bash
cd EmployeeBenefitsTracker/backend
dotnet test EmployeeBenefitsTracker.Tests --filter "ServiceTests" -v normal
```

Expected: 14 tests pass (9 employee + 5 benefit).

---

## Task 11: Controllers

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Api/Controllers/EmployeesController.cs`
- Create: `backend/EmployeeBenefitsTracker.Api/Controllers/BenefitsController.cs`

- [ ] **Step 1: Create EmployeesController**

```csharp
// Controllers/EmployeesController.cs
using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Services;
using EmployeeBenefitsTracker.Api.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeBenefitsTracker.Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? department = null,
        [FromQuery] string? benefitStatus = null)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search, department, benefitStatus);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try { return Ok(await _service.GetByIdAsync(id)); }
        catch (NotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        try { return Ok(await _service.UpdateAsync(id, dto)); }
        catch (NotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
        catch (NotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}
```

- [ ] **Step 2: Create BenefitsController**

```csharp
// Controllers/BenefitsController.cs
using EmployeeBenefitsTracker.Api.DTOs;
using EmployeeBenefitsTracker.Api.Services;
using EmployeeBenefitsTracker.Api.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeBenefitsTracker.Api.Controllers;

[ApiController]
[Route("api")]
public class BenefitsController : ControllerBase
{
    private readonly IBenefitEnrollmentService _service;

    public BenefitsController(IBenefitEnrollmentService service) => _service = service;

    [HttpGet("employees/{employeeId}/benefits")]
    public async Task<IActionResult> GetBenefits(int employeeId)
    {
        try { return Ok(await _service.GetByEmployeeIdAsync(employeeId)); }
        catch (NotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost("employees/{employeeId}/benefits")]
    public async Task<IActionResult> CreateBenefit(int employeeId, [FromBody] CreateBenefitEnrollmentDto dto)
    {
        try { return StatusCode(201, await _service.CreateAsync(employeeId, dto)); }
        catch (NotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("employees/{employeeId}/benefits/{benefitId}")]
    public async Task<IActionResult> UpdateBenefit(int employeeId, int benefitId, [FromBody] UpdateBenefitEnrollmentDto dto)
    {
        try { return Ok(await _service.UpdateAsync(employeeId, benefitId, dto)); }
        catch (NotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("employees/{employeeId}/benefits/{benefitId}")]
    public async Task<IActionResult> DeleteBenefit(int employeeId, int benefitId)
    {
        try { await _service.DeleteAsync(employeeId, benefitId); return NoContent(); }
        catch (NotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpGet("benefits/summary")]
    public async Task<IActionResult> GetSummary() => Ok(await _service.GetSummaryAsync());
}
```

---

## Task 12: Program.cs and Database Seeder

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Api/Data/DatabaseSeeder.cs`
- Create/Replace: `backend/EmployeeBenefitsTracker.Api/Program.cs`

- [ ] **Step 1: Create DatabaseSeeder**

```csharp
// Data/DatabaseSeeder.cs
using EmployeeBenefitsTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeBenefitsTracker.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Employees.AnyAsync()) return;

        var firstNames = new[] { "Alice", "Bob", "Carol", "David", "Emma", "Frank", "Grace", "Henry", "Iris", "Jack", "Karen", "Liam", "Mia", "Noah", "Olivia", "Peter", "Quinn", "Rachel", "Sam", "Tina", "Uma", "Victor", "Wendy", "Xander", "Yara", "Zoe" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Wilson", "Martinez", "Anderson", "Taylor", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Young", "Lewis" };
        var departments = new[] { "Engineering", "HR", "Finance", "Marketing", "Sales", "Operations", "Legal", "IT" };
        var rng = new Random(42);

        var employees = Enumerable.Range(1, 105).Select(i => new Employee
        {
            FirstName = firstNames[rng.Next(firstNames.Length)],
            LastName = lastNames[rng.Next(lastNames.Length)],
            Email = $"employee{i}@company.com",
            Department = departments[rng.Next(departments.Length)],
            EmploymentStatus = rng.Next(10) < 8 ? EmploymentStatus.Active : EmploymentStatus.Inactive,
            CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(365)),
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        context.Employees.AddRange(employees);
        await context.SaveChangesAsync();

        var benefitTypes = Enum.GetValues<BenefitType>();
        var statuses = Enum.GetValues<EnrollmentStatus>();
        var enrollments = new List<BenefitEnrollment>();

        foreach (var emp in employees)
        {
            var count = rng.Next(2, 6);
            var chosen = benefitTypes.OrderBy(_ => rng.Next()).Take(count);
            foreach (var type in chosen)
            {
                enrollments.Add(new BenefitEnrollment
                {
                    EmployeeId = emp.Id,
                    BenefitType = type,
                    EnrollmentStatus = statuses[rng.Next(statuses.Length)],
                    EffectiveDate = DateTime.UtcNow.AddDays(-rng.Next(180)),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        context.BenefitEnrollments.AddRange(enrollments);
        await context.SaveChangesAsync();
    }
}
```

- [ ] **Step 2: Write Program.cs**

```csharp
// Program.cs
using System.Text.Json;
using System.Text.Json.Serialization;
using EmployeeBenefitsTracker.Api.Data;
using EmployeeBenefitsTracker.Api.Repositories;
using EmployeeBenefitsTracker.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "benefits.db");
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IBenefitEnrollmentRepository, BenefitEnrollmentRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IBenefitEnrollmentService, BenefitEnrollmentService>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins(builder.Configuration["AllowedOrigins"] ?? "http://localhost:5173")
     .AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    ctx.Database.EnsureCreated();
    if (!app.Environment.IsEnvironment("Testing"))
        await DatabaseSeeder.SeedAsync(ctx);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();
app.Run();

public partial class Program { }
```

- [ ] **Step 3: Build and smoke-test the API**

```bash
cd EmployeeBenefitsTracker/backend/EmployeeBenefitsTracker.Api
dotnet run &
sleep 3
curl -s http://localhost:5000/api/employees | head -c 200
curl -s http://localhost:5000/api/benefits/summary
```

Expected: JSON response with paged employees and summary metrics.

---

## Task 13: Controller Tests

**Files:**
- Create: `backend/EmployeeBenefitsTracker.Tests/Controllers/EmployeesControllerTests.cs`
- Create: `backend/EmployeeBenefitsTracker.Tests/Controllers/BenefitsControllerTests.cs`

- [ ] **Step 1: Create EmployeesControllerTests**

```csharp
// Controllers/EmployeesControllerTests.cs
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
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedResult<EmployeeResponseDto>>(body, JsonOpts);
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
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedResult<EmployeeResponseDto>>(body, JsonOpts);
        Assert.NotNull(result);
        Assert.All(result.Items, e => Assert.Equal("Engineering", e.Department));
    }

    [Fact]
    public async Task CreateEmployee_ReturnsCreated()
    {
        var response = await CreateEmployee();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var emp = JsonSerializer.Deserialize<EmployeeResponseDto>(body, JsonOpts);
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
```

- [ ] **Step 2: Create BenefitsControllerTests**

```csharp
// Controllers/BenefitsControllerTests.cs
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
        var body = await response.Content.ReadAsStringAsync();
        var summary = JsonSerializer.Deserialize<BenefitSummaryDto>(body, JsonOpts);
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
```

- [ ] **Step 3: Run all tests**

```bash
cd EmployeeBenefitsTracker/backend
dotnet test -v normal
```

Expected: 21 tests pass.

---

## Task 14: Frontend Scaffold

**Files:**
- Create: `frontend/` (Vite project)
- Create: `frontend/.env`

- [ ] **Step 1: Scaffold Vite React TypeScript app**

```bash
cd EmployeeBenefitsTracker
npm create vite@latest frontend -- --template react-ts
cd frontend
npm install
```

- [ ] **Step 2: Install dependencies**

```bash
npm install @tanstack/react-query axios react-router-dom
npm install tailwindcss @tailwindcss/vite
```

- [ ] **Step 3: Configure Tailwind in vite.config.ts**

```typescript
// vite.config.ts
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [react(), tailwindcss()],
})
```

- [ ] **Step 4: Replace src/index.css**

```css
/* src/index.css */
@import "tailwindcss";
```

- [ ] **Step 5: Create .env**

```
VITE_API_BASE_URL=http://localhost:5000
```

- [ ] **Step 6: Verify frontend builds**

```bash
npm run build
```

Expected: Build succeeds with no TypeScript errors.

---

## Task 15: TypeScript Types and API Client

**Files:**
- Create: `frontend/src/types/employee.ts`
- Create: `frontend/src/types/benefit.ts`
- Create: `frontend/src/api/axios.ts`
- Create: `frontend/src/api/employees.ts`
- Create: `frontend/src/api/benefits.ts`

- [ ] **Step 1: Create employee types**

```typescript
// src/types/employee.ts
export type EmploymentStatus = 'Active' | 'Inactive';

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  department: string;
  employmentStatus: EmploymentStatus;
  createdAt: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  department: string;
  employmentStatus: EmploymentStatus;
}

export type UpdateEmployeeRequest = CreateEmployeeRequest;

export interface EmployeeFilters {
  page?: number;
  pageSize?: number;
  search?: string;
  department?: string;
  benefitStatus?: string;
}
```

- [ ] **Step 2: Create benefit types**

```typescript
// src/types/benefit.ts
export type BenefitType = 'Health' | 'Dental' | 'Vision' | 'Retirement' | 'Life';
export type EnrollmentStatus = 'NotEnrolled' | 'Pending' | 'Enrolled' | 'Waived';

export interface BenefitEnrollment {
  id: number;
  employeeId: number;
  benefitType: BenefitType;
  enrollmentStatus: EnrollmentStatus;
  effectiveDate: string | null;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBenefitRequest {
  benefitType: BenefitType;
  enrollmentStatus: EnrollmentStatus;
  effectiveDate?: string;
  notes?: string;
}

export interface UpdateBenefitRequest {
  enrollmentStatus: EnrollmentStatus;
  effectiveDate?: string;
  notes?: string;
}

export interface BenefitSummary {
  totalEmployees: number;
  activeEmployees: number;
  pendingEnrollments: number;
  enrolledByBenefitType: Record<string, number>;
  overallEnrollmentRate: number;
}
```

- [ ] **Step 3: Create Axios base client**

```typescript
// src/api/axios.ts
import axios from 'axios';

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

export default apiClient;
```

- [ ] **Step 4: Create employee API hooks**

```typescript
// src/api/employees.ts
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { CreateEmployeeRequest, Employee, EmployeeFilters, PagedResult, UpdateEmployeeRequest } from '../types/employee';
import apiClient from './axios';

const EMPLOYEES_KEY = 'employees';

export const useEmployees = (filters: EmployeeFilters = {}) =>
  useQuery<PagedResult<Employee>>({
    queryKey: [EMPLOYEES_KEY, filters],
    queryFn: async () => {
      const params = new URLSearchParams();
      if (filters.page) params.set('page', String(filters.page));
      if (filters.pageSize) params.set('pageSize', String(filters.pageSize));
      if (filters.search) params.set('search', filters.search);
      if (filters.department) params.set('department', filters.department);
      if (filters.benefitStatus) params.set('benefitStatus', filters.benefitStatus);
      const { data } = await apiClient.get(`/api/employees?${params}`);
      return data;
    },
  });

export const useEmployee = (id: number) =>
  useQuery<Employee>({
    queryKey: [EMPLOYEES_KEY, id],
    queryFn: async () => {
      const { data } = await apiClient.get(`/api/employees/${id}`);
      return data;
    },
  });

export const useCreateEmployee = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (dto: CreateEmployeeRequest) => {
      const { data } = await apiClient.post<Employee>('/api/employees', dto);
      return data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: [EMPLOYEES_KEY] }),
  });
};

export const useUpdateEmployee = (id: number) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (dto: UpdateEmployeeRequest) => {
      const { data } = await apiClient.put<Employee>(`/api/employees/${id}`, dto);
      return data;
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: [EMPLOYEES_KEY] });
      qc.invalidateQueries({ queryKey: [EMPLOYEES_KEY, id] });
    },
  });
};

export const useDeleteEmployee = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (id: number) => { await apiClient.delete(`/api/employees/${id}`); },
    onSuccess: () => qc.invalidateQueries({ queryKey: [EMPLOYEES_KEY] }),
  });
};
```

- [ ] **Step 5: Create benefit API hooks**

```typescript
// src/api/benefits.ts
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { BenefitEnrollment, BenefitSummary, CreateBenefitRequest, UpdateBenefitRequest } from '../types/benefit';
import apiClient from './axios';

const BENEFITS_KEY = 'benefits';
const SUMMARY_KEY = 'benefit-summary';

export const useEmployeeBenefits = (employeeId: number) =>
  useQuery<BenefitEnrollment[]>({
    queryKey: [BENEFITS_KEY, employeeId],
    queryFn: async () => {
      const { data } = await apiClient.get(`/api/employees/${employeeId}/benefits`);
      return data;
    },
  });

export const useBenefitSummary = () =>
  useQuery<BenefitSummary>({
    queryKey: [SUMMARY_KEY],
    queryFn: async () => {
      const { data } = await apiClient.get('/api/benefits/summary');
      return data;
    },
  });

export const useCreateBenefit = (employeeId: number) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (dto: CreateBenefitRequest) => {
      const { data } = await apiClient.post<BenefitEnrollment>(`/api/employees/${employeeId}/benefits`, dto);
      return data;
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: [BENEFITS_KEY, employeeId] });
      qc.invalidateQueries({ queryKey: [SUMMARY_KEY] });
    },
  });
};

export const useUpdateBenefit = (employeeId: number) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async ({ benefitId, dto }: { benefitId: number; dto: UpdateBenefitRequest }) => {
      const { data } = await apiClient.put<BenefitEnrollment>(`/api/employees/${employeeId}/benefits/${benefitId}`, dto);
      return data;
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: [BENEFITS_KEY, employeeId] }),
  });
};

export const useDeleteBenefit = (employeeId: number) => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: async (benefitId: number) => {
      await apiClient.delete(`/api/employees/${employeeId}/benefits/${benefitId}`);
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: [BENEFITS_KEY, employeeId] });
      qc.invalidateQueries({ queryKey: [SUMMARY_KEY] });
    },
  });
};
```

---

## Task 16: Reusable Components

**Files:**
- Create: `frontend/src/components/Badge.tsx`
- Create: `frontend/src/components/SummaryCard.tsx`
- Create: `frontend/src/components/Pagination.tsx`
- Create: `frontend/src/components/Modal.tsx`
- Create: `frontend/src/components/FormField.tsx`
- Create: `frontend/src/utils/formatters.ts`

- [ ] **Step 1: Create Badge**

```tsx
// src/components/Badge.tsx
interface BadgeProps {
  value: string;
}

const colorMap: Record<string, string> = {
  Active: 'bg-green-100 text-green-800',
  Inactive: 'bg-gray-100 text-gray-600',
  Enrolled: 'bg-blue-100 text-blue-800',
  Pending: 'bg-yellow-100 text-yellow-800',
  Waived: 'bg-purple-100 text-purple-800',
  NotEnrolled: 'bg-red-100 text-red-800',
};

export default function Badge({ value }: BadgeProps) {
  const classes = colorMap[value] ?? 'bg-gray-100 text-gray-600';
  const label = value === 'NotEnrolled' ? 'Not Enrolled' : value;
  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${classes}`}>
      {label}
    </span>
  );
}
```

- [ ] **Step 2: Create SummaryCard**

```tsx
// src/components/SummaryCard.tsx
interface SummaryCardProps {
  title: string;
  value: string | number;
  subtitle?: string;
}

export default function SummaryCard({ title, value, subtitle }: SummaryCardProps) {
  return (
    <div className="bg-white rounded-lg border border-gray-200 p-5 shadow-sm">
      <p className="text-sm font-medium text-gray-500">{title}</p>
      <p className="mt-1 text-3xl font-semibold text-gray-900">{value}</p>
      {subtitle && <p className="mt-1 text-sm text-gray-400">{subtitle}</p>}
    </div>
  );
}
```

- [ ] **Step 3: Create Pagination**

```tsx
// src/components/Pagination.tsx
interface PaginationProps {
  page: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export default function Pagination({ page, totalPages, onPageChange }: PaginationProps) {
  if (totalPages <= 1) return null;
  return (
    <div className="flex items-center justify-between mt-4">
      <p className="text-sm text-gray-600">Page {page} of {totalPages}</p>
      <div className="flex gap-2">
        <button
          onClick={() => onPageChange(page - 1)}
          disabled={page <= 1}
          className="px-3 py-1 text-sm border rounded disabled:opacity-40 hover:bg-gray-50"
        >
          Previous
        </button>
        <button
          onClick={() => onPageChange(page + 1)}
          disabled={page >= totalPages}
          className="px-3 py-1 text-sm border rounded disabled:opacity-40 hover:bg-gray-50"
        >
          Next
        </button>
      </div>
    </div>
  );
}
```

- [ ] **Step 4: Create Modal**

```tsx
// src/components/Modal.tsx
interface ModalProps {
  title: string;
  children: React.ReactNode;
  onClose: () => void;
}

export default function Modal({ title, children, onClose }: ModalProps) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-md mx-4">
        <div className="flex items-center justify-between px-6 py-4 border-b">
          <h2 className="text-lg font-semibold text-gray-800">{title}</h2>
          <button onClick={onClose} className="text-gray-400 hover:text-gray-600 text-xl leading-none">&times;</button>
        </div>
        <div className="px-6 py-4">{children}</div>
      </div>
    </div>
  );
}
```

- [ ] **Step 5: Create FormField**

```tsx
// src/components/FormField.tsx
interface FormFieldProps {
  label: string;
  error?: string;
  children: React.ReactNode;
}

export function FormField({ label, error, children }: FormFieldProps) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">{label}</label>
      {children}
      {error && <p className="mt-1 text-xs text-red-600">{error}</p>}
    </div>
  );
}

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  error?: boolean;
}

export function Input({ error, ...props }: InputProps) {
  return (
    <input
      {...props}
      className={`w-full px-3 py-2 border rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 ${error ? 'border-red-400' : 'border-gray-300'} ${props.className ?? ''}`}
    />
  );
}

interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
  options: { value: string; label: string }[];
  error?: boolean;
}

export function Select({ options, error, ...props }: SelectProps) {
  return (
    <select
      {...props}
      className={`w-full px-3 py-2 border rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 ${error ? 'border-red-400' : 'border-gray-300'} ${props.className ?? ''}`}
    >
      {options.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}
    </select>
  );
}
```

- [ ] **Step 6: Create formatters**

```typescript
// src/utils/formatters.ts
export const formatDate = (iso: string | null | undefined): string => {
  if (!iso) return '—';
  return new Date(iso).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
};

export const formatPercent = (rate: number): string => `${(rate * 100).toFixed(1)}%`;

export const DEPARTMENTS = ['Engineering', 'HR', 'Finance', 'Marketing', 'Sales', 'Operations', 'Legal', 'IT'];
export const BENEFIT_TYPES = ['Health', 'Dental', 'Vision', 'Retirement', 'Life'] as const;
export const ENROLLMENT_STATUSES = ['NotEnrolled', 'Pending', 'Enrolled', 'Waived'] as const;
export const EMPLOYMENT_STATUSES = ['Active', 'Inactive'] as const;
```

---

## Task 17: EmployeeList Page

**Files:**
- Create: `frontend/src/pages/EmployeeList.tsx`

- [ ] **Step 1: Create EmployeeList page**

```tsx
// src/pages/EmployeeList.tsx
import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useDeleteEmployee, useEmployees } from '../api/employees';
import { useBenefitSummary } from '../api/benefits';
import Badge from '../components/Badge';
import Pagination from '../components/Pagination';
import SummaryCard from '../components/SummaryCard';
import { formatPercent, DEPARTMENTS, ENROLLMENT_STATUSES } from '../utils/formatters';

export default function EmployeeList() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [department, setDepartment] = useState('');
  const [benefitStatus, setBenefitStatus] = useState('');
  const [deleteConfirm, setDeleteConfirm] = useState<number | null>(null);

  const { data, isLoading, error } = useEmployees({ page, pageSize: 10, search: search || undefined, department: department || undefined, benefitStatus: benefitStatus || undefined });
  const { data: summary } = useBenefitSummary();
  const deleteMutation = useDeleteEmployee();

  const handleSearch = (v: string) => { setSearch(v); setPage(1); };
  const handleDept = (v: string) => { setDepartment(v); setPage(1); };
  const handleStatus = (v: string) => { setBenefitStatus(v); setPage(1); };

  const confirmDelete = async (id: number) => {
    await deleteMutation.mutateAsync(id);
    setDeleteConfirm(null);
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-8">
        <div className="flex items-center justify-between mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Employee Benefits Tracker</h1>
          <Link to="/employees/new" className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700">
            + Add Employee
          </Link>
        </div>

        {summary && (
          <div className="grid grid-cols-2 md:grid-cols-5 gap-4 mb-8">
            <SummaryCard title="Total Employees" value={summary.totalEmployees} />
            <SummaryCard title="Active Employees" value={summary.activeEmployees} />
            <SummaryCard title="Health Enrolled" value={summary.enrolledByBenefitType['Health'] ?? 0} />
            <SummaryCard title="Pending Enrollments" value={summary.pendingEnrollments} />
            <SummaryCard title="Enrollment Rate" value={formatPercent(summary.overallEnrollmentRate)} />
          </div>
        )}

        <div className="bg-white rounded-xl border border-gray-200 shadow-sm">
          <div className="p-4 border-b flex flex-wrap gap-3">
            <input
              type="text"
              placeholder="Search name or email..."
              value={search}
              onChange={e => handleSearch(e.target.value)}
              className="flex-1 min-w-48 px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <select value={department} onChange={e => handleDept(e.target.value)} className="px-3 py-2 border border-gray-300 rounded-md text-sm">
              <option value="">All Departments</option>
              {DEPARTMENTS.map(d => <option key={d} value={d}>{d}</option>)}
            </select>
            <select value={benefitStatus} onChange={e => handleStatus(e.target.value)} className="px-3 py-2 border border-gray-300 rounded-md text-sm">
              <option value="">All Statuses</option>
              {ENROLLMENT_STATUSES.map(s => <option key={s} value={s}>{s === 'NotEnrolled' ? 'Not Enrolled' : s}</option>)}
            </select>
          </div>

          {isLoading && <div className="p-8 text-center text-gray-500">Loading...</div>}
          {error && <div className="p-8 text-center text-red-500">Failed to load employees.</div>}

          {data && (
            <>
              <table className="w-full text-sm">
                <thead className="bg-gray-50 text-gray-600 uppercase text-xs tracking-wide">
                  <tr>
                    {['Name', 'Email', 'Department', 'Status', 'Actions'].map(h => (
                      <th key={h} className="px-4 py-3 text-left font-medium">{h}</th>
                    ))}
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {data.items.length === 0 && (
                    <tr><td colSpan={5} className="px-4 py-8 text-center text-gray-400">No employees found.</td></tr>
                  )}
                  {data.items.map(emp => (
                    <tr key={emp.id} className="hover:bg-gray-50 transition-colors">
                      <td className="px-4 py-3 font-medium text-gray-900">
                        <Link to={`/employees/${emp.id}`} className="hover:text-blue-600">
                          {emp.firstName} {emp.lastName}
                        </Link>
                      </td>
                      <td className="px-4 py-3 text-gray-600">{emp.email}</td>
                      <td className="px-4 py-3 text-gray-600">{emp.department}</td>
                      <td className="px-4 py-3"><Badge value={emp.employmentStatus} /></td>
                      <td className="px-4 py-3">
                        <div className="flex gap-2">
                          <Link to={`/employees/${emp.id}/edit`} className="text-blue-600 hover:underline text-xs">Edit</Link>
                          {deleteConfirm === emp.id ? (
                            <span className="flex gap-1">
                              <button onClick={() => confirmDelete(emp.id)} className="text-red-600 hover:underline text-xs">Confirm</button>
                              <button onClick={() => setDeleteConfirm(null)} className="text-gray-400 hover:underline text-xs">Cancel</button>
                            </span>
                          ) : (
                            <button onClick={() => setDeleteConfirm(emp.id)} className="text-red-400 hover:text-red-600 text-xs">Delete</button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              <div className="px-4 pb-4">
                <Pagination page={data.page} totalPages={data.totalPages} onPageChange={setPage} />
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
```

---

## Task 18: EmployeeForm Page

**Files:**
- Create: `frontend/src/pages/EmployeeForm.tsx`

- [ ] **Step 1: Create EmployeeForm page**

```tsx
// src/pages/EmployeeForm.tsx
import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useCreateEmployee, useEmployee, useUpdateEmployee } from '../api/employees';
import { FormField, Input, Select } from '../components/FormField';
import { DEPARTMENTS, EMPLOYMENT_STATUSES } from '../utils/formatters';
import type { CreateEmployeeRequest } from '../types/employee';

const empty: CreateEmployeeRequest = { firstName: '', lastName: '', email: '', department: 'Engineering', employmentStatus: 'Active' };

export default function EmployeeForm() {
  const { id } = useParams();
  const isEdit = Boolean(id);
  const navigate = useNavigate();
  const [form, setForm] = useState<CreateEmployeeRequest>(empty);
  const [errors, setErrors] = useState<Partial<Record<keyof CreateEmployeeRequest | 'api', string>>>({});

  const { data: existing } = useEmployee(Number(id));
  const createMutation = useCreateEmployee();
  const updateMutation = useUpdateEmployee(Number(id));

  useEffect(() => {
    if (existing && isEdit) {
      setForm({ firstName: existing.firstName, lastName: existing.lastName, email: existing.email, department: existing.department, employmentStatus: existing.employmentStatus });
    }
  }, [existing, isEdit]);

  const validate = (): boolean => {
    const errs: typeof errors = {};
    if (!form.firstName.trim()) errs.firstName = 'First name is required.';
    if (!form.lastName.trim()) errs.lastName = 'Last name is required.';
    if (!form.email.trim()) errs.email = 'Email is required.';
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) errs.email = 'Invalid email format.';
    if (!form.department) errs.department = 'Department is required.';
    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;
    try {
      if (isEdit) {
        await updateMutation.mutateAsync(form);
        navigate(`/employees/${id}`);
      } else {
        const created = await createMutation.mutateAsync(form);
        navigate(`/employees/${created.id}`);
      }
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message ?? 'An error occurred.';
      setErrors(prev => ({ ...prev, api: msg }));
    }
  };

  const set = (field: keyof CreateEmployeeRequest) => (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
    setForm(prev => ({ ...prev, [field]: e.target.value }));

  const isPending = createMutation.isPending || updateMutation.isPending;

  return (
    <div className="min-h-screen bg-gray-50 flex items-start justify-center pt-12 px-4">
      <div className="bg-white rounded-xl border border-gray-200 shadow-sm w-full max-w-lg p-8">
        <h1 className="text-xl font-semibold text-gray-900 mb-6">{isEdit ? 'Edit Employee' : 'Add Employee'}</h1>

        {errors.api && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded text-sm text-red-700">{errors.api}</div>}

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <FormField label="First Name" error={errors.firstName}>
              <Input value={form.firstName} onChange={set('firstName')} error={Boolean(errors.firstName)} />
            </FormField>
            <FormField label="Last Name" error={errors.lastName}>
              <Input value={form.lastName} onChange={set('lastName')} error={Boolean(errors.lastName)} />
            </FormField>
          </div>
          <FormField label="Email" error={errors.email}>
            <Input type="email" value={form.email} onChange={set('email')} error={Boolean(errors.email)} />
          </FormField>
          <FormField label="Department" error={errors.department}>
            <Select value={form.department} onChange={set('department')} options={DEPARTMENTS.map(d => ({ value: d, label: d }))} />
          </FormField>
          <FormField label="Status">
            <Select value={form.employmentStatus} onChange={set('employmentStatus')} options={EMPLOYMENT_STATUSES.map(s => ({ value: s, label: s }))} />
          </FormField>

          <div className="flex gap-3 pt-2">
            <button type="button" onClick={() => navigate(-1)} className="flex-1 px-4 py-2 border border-gray-300 rounded-lg text-sm text-gray-700 hover:bg-gray-50">
              Cancel
            </button>
            <button type="submit" disabled={isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-700 disabled:opacity-50">
              {isPending ? 'Saving...' : isEdit ? 'Save Changes' : 'Create Employee'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
```

---

## Task 19: EmployeeDetail Page

**Files:**
- Create: `frontend/src/pages/EmployeeDetail.tsx`

- [ ] **Step 1: Create EmployeeDetail page**

```tsx
// src/pages/EmployeeDetail.tsx
import { useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { useEmployee } from '../api/employees';
import { useCreateBenefit, useDeleteBenefit, useEmployeeBenefits, useUpdateBenefit } from '../api/benefits';
import Badge from '../components/Badge';
import Modal from '../components/Modal';
import { FormField, Select } from '../components/FormField';
import { BENEFIT_TYPES, ENROLLMENT_STATUSES, formatDate } from '../utils/formatters';
import type { BenefitType, CreateBenefitRequest, EnrollmentStatus, UpdateBenefitRequest } from '../types/benefit';

export default function EmployeeDetail() {
  const { id } = useParams();
  const empId = Number(id);
  const { data: employee, isLoading } = useEmployee(empId);
  const { data: benefits } = useEmployeeBenefits(empId);
  const createBenefit = useCreateBenefit(empId);
  const updateBenefit = useUpdateBenefit(empId);
  const deleteBenefit = useDeleteBenefit(empId);

  const [showAdd, setShowAdd] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [newBenefit, setNewBenefit] = useState<CreateBenefitRequest>({ benefitType: 'Health', enrollmentStatus: 'Pending' });
  const [editStatus, setEditStatus] = useState<EnrollmentStatus>('Pending');
  const [addError, setAddError] = useState('');

  const existingTypes = new Set(benefits?.map(b => b.benefitType) ?? []);
  const availableTypes = BENEFIT_TYPES.filter(t => !existingTypes.has(t));

  const handleAddBenefit = async () => {
    setAddError('');
    try {
      await createBenefit.mutateAsync(newBenefit);
      setShowAdd(false);
      setNewBenefit({ benefitType: 'Health', enrollmentStatus: 'Pending' });
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })?.response?.data?.message ?? 'Failed to add benefit.';
      setAddError(msg);
    }
  };

  const handleUpdateBenefit = async () => {
    if (!editId) return;
    await updateBenefit.mutateAsync({ benefitId: editId, dto: { enrollmentStatus: editStatus } });
    setEditId(null);
  };

  if (isLoading) return <div className="p-8 text-center text-gray-500">Loading...</div>;
  if (!employee) return <div className="p-8 text-center text-red-500">Employee not found.</div>;

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-4xl mx-auto px-4 py-8">
        <div className="flex items-center gap-3 mb-6">
          <Link to="/" className="text-gray-400 hover:text-gray-600 text-sm">← Back</Link>
        </div>

        <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6 mb-6">
          <div className="flex items-start justify-between">
            <div>
              <h1 className="text-2xl font-bold text-gray-900">{employee.firstName} {employee.lastName}</h1>
              <p className="text-gray-500 mt-1">{employee.email}</p>
            </div>
            <Link to={`/employees/${empId}/edit`} className="px-4 py-2 border border-gray-300 rounded-lg text-sm text-gray-700 hover:bg-gray-50">Edit</Link>
          </div>
          <div className="mt-4 grid grid-cols-2 gap-4 text-sm">
            <div><span className="text-gray-500">Department:</span> <span className="font-medium ml-1">{employee.department}</span></div>
            <div><span className="text-gray-500">Status:</span> <span className="ml-1"><Badge value={employee.employmentStatus} /></span></div>
            <div><span className="text-gray-500">Joined:</span> <span className="font-medium ml-1">{formatDate(employee.createdAt)}</span></div>
          </div>
        </div>

        <div className="bg-white rounded-xl border border-gray-200 shadow-sm">
          <div className="flex items-center justify-between px-6 py-4 border-b">
            <h2 className="text-lg font-semibold text-gray-800">Benefits Enrollment</h2>
            {availableTypes.length > 0 && (
              <button onClick={() => { setShowAdd(true); setNewBenefit({ benefitType: availableTypes[0], enrollmentStatus: 'Pending' }); }}
                className="bg-blue-600 text-white px-3 py-1.5 rounded-lg text-sm hover:bg-blue-700">
                + Add Benefit
              </button>
            )}
          </div>

          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-gray-600 uppercase text-xs tracking-wide">
              <tr>
                {['Benefit Type', 'Status', 'Effective Date', 'Notes', 'Actions'].map(h => (
                  <th key={h} className="px-4 py-3 text-left font-medium">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {(!benefits || benefits.length === 0) && (
                <tr><td colSpan={5} className="px-4 py-6 text-center text-gray-400">No benefits enrolled.</td></tr>
              )}
              {benefits?.map(b => (
                <tr key={b.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium">{b.benefitType}</td>
                  <td className="px-4 py-3"><Badge value={b.enrollmentStatus} /></td>
                  <td className="px-4 py-3 text-gray-500">{formatDate(b.effectiveDate)}</td>
                  <td className="px-4 py-3 text-gray-500 max-w-xs truncate">{b.notes ?? '—'}</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button onClick={() => { setEditId(b.id); setEditStatus(b.enrollmentStatus as EnrollmentStatus); }}
                        className="text-blue-600 hover:underline text-xs">Edit</button>
                      <button onClick={() => deleteBenefit.mutate(b.id)} className="text-red-400 hover:text-red-600 text-xs">Remove</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {showAdd && (
        <Modal title="Add Benefit" onClose={() => setShowAdd(false)}>
          <div className="space-y-4">
            {addError && <p className="text-sm text-red-600">{addError}</p>}
            <FormField label="Benefit Type">
              <Select value={newBenefit.benefitType} onChange={e => setNewBenefit(p => ({ ...p, benefitType: e.target.value as BenefitType }))}
                options={availableTypes.map(t => ({ value: t, label: t }))} />
            </FormField>
            <FormField label="Status">
              <Select value={newBenefit.enrollmentStatus} onChange={e => setNewBenefit(p => ({ ...p, enrollmentStatus: e.target.value as EnrollmentStatus }))}
                options={ENROLLMENT_STATUSES.map(s => ({ value: s, label: s === 'NotEnrolled' ? 'Not Enrolled' : s }))} />
            </FormField>
            <div className="flex gap-3 pt-2">
              <button onClick={() => setShowAdd(false)} className="flex-1 px-4 py-2 border rounded-lg text-sm text-gray-700">Cancel</button>
              <button onClick={handleAddBenefit} disabled={createBenefit.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm disabled:opacity-50">
                {createBenefit.isPending ? 'Adding...' : 'Add'}
              </button>
            </div>
          </div>
        </Modal>
      )}

      {editId && (
        <Modal title="Update Benefit Status" onClose={() => setEditId(null)}>
          <div className="space-y-4">
            <FormField label="Status">
              <Select value={editStatus} onChange={e => setEditStatus(e.target.value as EnrollmentStatus)}
                options={ENROLLMENT_STATUSES.map(s => ({ value: s, label: s === 'NotEnrolled' ? 'Not Enrolled' : s }))} />
            </FormField>
            <div className="flex gap-3 pt-2">
              <button onClick={() => setEditId(null)} className="flex-1 px-4 py-2 border rounded-lg text-sm text-gray-700">Cancel</button>
              <button onClick={handleUpdateBenefit} disabled={updateBenefit.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm disabled:opacity-50">
                {updateBenefit.isPending ? 'Saving...' : 'Save'}
              </button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
```

---

## Task 20: App.tsx and main.tsx

**Files:**
- Replace: `frontend/src/App.tsx`
- Replace: `frontend/src/main.tsx`

- [ ] **Step 1: Write App.tsx with routing**

```tsx
// src/App.tsx
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import EmployeeList from './pages/EmployeeList';
import EmployeeForm from './pages/EmployeeForm';
import EmployeeDetail from './pages/EmployeeDetail';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<EmployeeList />} />
        <Route path="/employees/new" element={<EmployeeForm />} />
        <Route path="/employees/:id" element={<EmployeeDetail />} />
        <Route path="/employees/:id/edit" element={<EmployeeForm />} />
      </Routes>
    </BrowserRouter>
  );
}
```

- [ ] **Step 2: Write main.tsx**

```tsx
// src/main.tsx
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import './index.css';
import App from './App';

const queryClient = new QueryClient({
  defaultOptions: { queries: { retry: 1, staleTime: 30_000 } },
});

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <App />
    </QueryClientProvider>
  </StrictMode>
);
```

- [ ] **Step 3: Run full TypeScript build to confirm no errors**

```bash
cd EmployeeBenefitsTracker/frontend
npm run build
```

Expected: Build succeeds, 0 TypeScript errors.

---

## Task 21: GitHub Actions CI

**Files:**
- Create: `.github/workflows/ci.yml`

- [ ] **Step 1: Create CI workflow**

```yaml
# .github/workflows/ci.yml
name: CI

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  backend:
    name: Backend — Build & Test
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: backend

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET 8
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore EmployeeBenefitsTracker.sln

      - name: Build
        run: dotnet build EmployeeBenefitsTracker.sln --no-restore --configuration Release

      - name: Run tests
        run: dotnet test EmployeeBenefitsTracker.sln --no-build --configuration Release --verbosity normal

  frontend:
    name: Frontend — Type Check & Build
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: frontend

    steps:
      - uses: actions/checkout@v4

      - name: Setup Node 20
        uses: actions/setup-node@v4
        with:
          node-version: '20'
          cache: 'npm'
          cache-dependency-path: frontend/package-lock.json

      - name: Install dependencies
        run: npm ci

      - name: Build (type-check + bundle)
        run: npm run build
        env:
          VITE_API_BASE_URL: http://localhost:5000
```

---

## Task 22: README

**Files:**
- Create: `README.md`

- [ ] **Step 1: Write README**

````markdown
# Employee Benefits Tracker

A full-stack HR tool for managing employee records and benefits enrollment status. Built with C#, ASP.NET Core, React TypeScript, and SQLite.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend API | C# / ASP.NET Core 8 / Entity Framework Core |
| Database | SQLite (auto-created, no migrations) |
| Testing | xUnit / WebApplicationFactory / SQLite in-memory |
| Frontend | React 18 / TypeScript / Vite / Tailwind CSS v4 |
| State | React Query v5 / Axios |
| CI/CD | GitHub Actions |

## Architecture

```
EmployeeBenefitsTracker/
├── backend/
│   ├── EmployeeBenefitsTracker.Api/       # ASP.NET Core Web API
│   │   ├── Controllers/                    # HTTP layer
│   │   ├── Services/                       # Business logic + validation
│   │   ├── Repositories/                   # EF Core data access
│   │   ├── Models/                         # Entity classes + enums
│   │   ├── DTOs/                           # Request/response shapes
│   │   └── Data/                           # DbContext + seeder
│   └── EmployeeBenefitsTracker.Tests/      # xUnit tests
└── frontend/                               # Vite React TypeScript app
```

## Core Features

- Full CRUD for 100+ employees (create, view, edit, delete)
- Benefits enrollment management per employee (Health, Dental, Vision, Retirement, Life)
- Search, department filter, and benefit status filter with pagination
- Dashboard summary cards: total/active employees, pending enrollments, enrollment rate
- Inline add/update/delete benefit enrollments per employee

## API Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/employees` | Paged list (`?page=1&pageSize=10&search=&department=&benefitStatus=`) |
| GET | `/api/employees/{id}` | Single employee |
| POST | `/api/employees` | Create employee |
| PUT | `/api/employees/{id}` | Update employee |
| DELETE | `/api/employees/{id}` | Delete employee |
| GET | `/api/employees/{id}/benefits` | Employee's benefit enrollments |
| POST | `/api/employees/{id}/benefits` | Add benefit enrollment |
| PUT | `/api/employees/{empId}/benefits/{benefitId}` | Update benefit status |
| DELETE | `/api/employees/{empId}/benefits/{benefitId}` | Remove benefit enrollment |
| GET | `/api/benefits/summary` | Dashboard metrics |

## Database Schema

**Employees:** Id, FirstName, LastName, Email (unique), Department, EmploymentStatus, CreatedAt, UpdatedAt

**BenefitEnrollments:** Id, EmployeeId (FK), BenefitType, EnrollmentStatus, EffectiveDate, Notes, CreatedAt, UpdatedAt

Constraint: one enrollment record per BenefitType per employee. Cascade delete on employee removal.

> EF Core migrations are intentionally skipped for this portfolio project. The database is created automatically via `EnsureCreated()` on first run, with 105 seeded employees and realistic benefit data.

## Running the Backend

```bash
cd backend/EmployeeBenefitsTracker.Api
dotnet run
# API available at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger
```

## Running the Frontend

```bash
cd frontend
npm install
npm run dev
# App available at http://localhost:5173
```

## Running Tests

```bash
cd backend
dotnet test --verbosity normal
# 21 tests: service tests (SQLite in-memory) + controller tests (WebApplicationFactory)
```

## CI/CD

GitHub Actions runs two parallel jobs on every push and pull request to `main`:

- **Backend:** restore → build → run 21 xUnit tests
- **Frontend:** `npm ci` → `npm run build` (TypeScript type-check + Vite bundle)

## Resume Bullets

- Built C# ASP.NET Core REST APIs for 5 benefits workflows using EF Core and SQLite persistence.
- Developed React TypeScript dashboard with search, pagination, filters, and React Query across 100+ records.
- Added 20+ xUnit tests covering CRUD, validation, cascade deletes, filtering, and API error paths.
- Configured GitHub Actions CI validating .NET tests and React TypeScript builds across 2 parallel jobs.
````

- [ ] **Step 2: Verify the full project**

```bash
# Terminal 1: start backend
cd EmployeeBenefitsTracker/backend/EmployeeBenefitsTracker.Api && dotnet run

# Terminal 2: start frontend
cd EmployeeBenefitsTracker/frontend && npm run dev

# Terminal 3: run all tests
cd EmployeeBenefitsTracker/backend && dotnet test -v normal
```

Expected: 21 tests pass, API responds at localhost:5000, frontend loads at localhost:5173 with employee data visible.

---

## Self-Review: Spec Coverage Check

| Spec Section | Covered By |
|---|---|
| Employee CRUD (5 operations) | Tasks 9, 11 |
| Benefits enrollment (5 benefit types, 4 statuses) | Tasks 10, 11 |
| DELETE benefit endpoint | Task 11 (BenefitsController) |
| Pagination with `PagedResult<T>` | Tasks 3, 6, 9 |
| Search + department + benefitStatus filters | Tasks 6, 17 |
| Case-insensitive duplicate email | Tasks 8, 9 (GetByEmailAsync uses `.ToLower()`) |
| One enrollment per BenefitType per employee | Tasks 4 (DB index), 10 (service check) |
| `/api/benefits/summary` with all 5 metrics | Tasks 6, 11 |
| `EnsureCreated()` + seeder (100+ employees) | Task 12 |
| 21 xUnit tests (14 service + 7 controller) | Tasks 8–10, 13 |
| SQLite in-memory for all tests | Tasks 7, 13 |
| WebApplicationFactory with Testing env | Tasks 7, 13 |
| React Query for all server state | Tasks 14–20 |
| Tailwind CSS v4 | Task 14 |
| `VITE_API_BASE_URL` env variable | Tasks 14, 15 |
| EmployeeList with summary cards + filters | Task 17 |
| EmployeeForm with validation + API error surface | Task 18 |
| EmployeeDetail with inline benefit management | Task 19 |
| React Router with 4 routes | Task 20 |
| GitHub Actions (2 parallel jobs) | Task 21 |
| README with all required sections | Task 22 |
````
