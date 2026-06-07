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
