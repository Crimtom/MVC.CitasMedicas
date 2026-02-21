using API.CitasMedicas.Services;
using DTO.Appointment;
using DTO.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.CitasMedicas.Controllers;

/// <summary>
/// BFF Controller - Proxies requests to API.CitasMedicas microservice
/// </summary>
[ApiController]
[Route("api/citas")]
[Tags("Appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentApiService _apiService;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IAppointmentApiService apiService, ILogger<AppointmentsController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    /// <summary>
    /// Get all appointments, optionally filtered by date.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AppointmentDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetAll([FromQuery] DateTime? date)
    {
        var appointments = await _apiService.GetAllAsync(date);
        return Ok(ApiResponse<List<AppointmentDto>>.Ok(appointments, "Appointments retrieved"));
    }

    /// <summary>
    /// Get a single appointment by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 404)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetById(int id)
    {
        var appointment = await _apiService.GetByIdAsync(id);
        if (appointment is null)
            return NotFound(ApiResponse<AppointmentDto>.Fail("Appointment not found"));

        return Ok(ApiResponse<AppointmentDto>.Ok(appointment, "Appointment retrieved"));
    }

    /// <summary>
    /// Create a new appointment.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 400)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Create([FromBody] AppointmentCreateDto dto)
    {
        var (success, message, data) = await _apiService.CreateAsync(dto);
        
        if (!success)
            return BadRequest(ApiResponse<AppointmentDto>.Fail(message));

        return CreatedAtAction(nameof(GetById), new { id = data!.Id },
            ApiResponse<AppointmentDto>.Ok(data, message));
    }

    /// <summary>
    /// Update an existing appointment.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 404)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Update(int id, [FromBody] AppointmentUpdateDto dto)
    {
        var (success, message, data) = await _apiService.UpdateAsync(id, dto);
        
        if (!success)
        {
            if (message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(ApiResponse<AppointmentDto>.Fail(message));
            return BadRequest(ApiResponse<AppointmentDto>.Fail(message));
        }

        return Ok(ApiResponse<AppointmentDto>.Ok(data, message));
    }

    /// <summary>
    /// Cancel an appointment (soft delete).
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var (success, message) = await _apiService.DeleteAsync(id);
        
        if (!success)
            return NotFound(ApiResponse<object>.Fail(message));

        return Ok(ApiResponse<object>.Ok(null, message));
    }
}
