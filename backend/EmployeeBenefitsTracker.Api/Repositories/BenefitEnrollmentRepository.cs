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
        await _context.BenefitEnrollments.Where(b => b.EmployeeId == employeeId).AsNoTracking().ToListAsync();

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

        var enrolledGroups = await _context.BenefitEnrollments
            .Where(b => b.EnrollmentStatus == EnrollmentStatus.Enrolled)
            .GroupBy(b => b.BenefitType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        var enrolledByType = Enum.GetValues<BenefitType>()
            .ToDictionary(t => t.ToString(), t => enrolledGroups.FirstOrDefault(e => e.Type == t)?.Count ?? 0);

        var totalEnrolled = enrolledByType.Values.Sum();
        var benefitTypeCount = Enum.GetValues<BenefitType>().Length;
        var rate = totalEmployees > 0 ? Math.Round((double)totalEnrolled / (totalEmployees * benefitTypeCount), 4) : 0;

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
