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
            .Property(e => e.Email)
            .UseCollation("NOCASE");

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
