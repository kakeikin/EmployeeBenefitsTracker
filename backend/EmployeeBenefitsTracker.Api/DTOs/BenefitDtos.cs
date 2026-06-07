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
