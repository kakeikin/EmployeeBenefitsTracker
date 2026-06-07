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
