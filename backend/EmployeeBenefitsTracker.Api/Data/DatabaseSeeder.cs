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
