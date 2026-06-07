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
