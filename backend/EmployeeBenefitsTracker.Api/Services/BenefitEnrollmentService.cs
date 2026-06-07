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
