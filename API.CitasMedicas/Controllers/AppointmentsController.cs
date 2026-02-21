using Business.Interfaces;
using DTO.Appointment;
using DTO.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.CitasMedicas.Controllers;

[ApiController]
[Route("api/citas")]
[Tags("Appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentBusiness _business;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IAppointmentBusiness business, ILogger<AppointmentsController> logger)
    {
        _business = business;
        _logger = logger;
    }

    /// <summary>
    /// Get all appointments, optionally filtered by date.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AppointmentDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetAll([FromQuery] DateTime? date)
    {
        // Validación
        try
        {
            DateOnly? dateFilter = date.HasValue ? DateOnly.FromDateTime(date.Value) : null;
            var appointments = await _business.GetAll(dateFilter); // DTO Appointment
            return Ok(ApiResponse<List<AppointmentDto>>.Ok(appointments, "Appointments retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointments");
            return StatusCode(500, ApiResponse<List<AppointmentDto>>.Fail("Unexpected error"));
        }
    }

    /// <summary>
    /// Get a single appointment by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 404)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetById(int id)
    {
        try
        {
            var appointment = await _business.GetById(id);
            if (appointment is null)
                return NotFound(ApiResponse<AppointmentDto>.Fail("Appointment not found"));

            return Ok(ApiResponse<AppointmentDto>.Ok(appointment, "Appointment retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment {Id}", id);
            return StatusCode(500, ApiResponse<AppointmentDto>.Fail("Unexpected error"));
        }
    }

    /// <summary>
    /// Create a new appointment.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 404)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 409)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Create([FromBody] AppointmentCreateDto dto)
    {

        // Validaciones
        try
        {
            var validation = ValidateInput(dto.PatientId, dto.DoctorId, dto.AppointmentDateTime);
            if (validation is not null)
                return BadRequest(ApiResponse<AppointmentDto>.Fail(validation));

            if (!await _business.PatientExists(dto.PatientId))
                return NotFound(ApiResponse<AppointmentDto>.Fail("Patient not found"));

            if (!await _business.DoctorExists(dto.DoctorId))
                return NotFound(ApiResponse<AppointmentDto>.Fail("Doctor not found"));

            var date = DateOnly.FromDateTime(dto.AppointmentDateTime);
            var time = TimeOnly.FromDateTime(dto.AppointmentDateTime);

            if (await _business.HasConflict(dto.DoctorId, date, time))
                return Conflict(ApiResponse<AppointmentDto>.Fail("Doctor already has an appointment at that time"));

            var created = await _business.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                ApiResponse<AppointmentDto>.Ok(created, "Appointment created"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            return StatusCode(500, ApiResponse<AppointmentDto>.Fail("Unexpected error"));
        }
    }

    /// <summary>
    /// Update an existing appointment.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 400)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 404)]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), 409)]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Update(int id, [FromBody] AppointmentUpdateDto dto)
    {
        try
        {
            var validation = ValidateInput(dto.PatientId, dto.DoctorId, dto.AppointmentDateTime);
            if (validation is not null)
                return BadRequest(ApiResponse<AppointmentDto>.Fail(validation));

            if (!await _business.PatientExists(dto.PatientId))
                return NotFound(ApiResponse<AppointmentDto>.Fail("Patient not found"));

            if (!await _business.DoctorExists(dto.DoctorId))
                return NotFound(ApiResponse<AppointmentDto>.Fail("Doctor not found"));

            var date = DateOnly.FromDateTime(dto.AppointmentDateTime);
            var time = TimeOnly.FromDateTime(dto.AppointmentDateTime);

            if (await _business.HasConflict(dto.DoctorId, date, time, excludeId: id))
                return Conflict(ApiResponse<AppointmentDto>.Fail("Doctor already has an appointment at that time"));

            var updated = await _business.Update(id, dto);
            if (updated is null)
                return NotFound(ApiResponse<AppointmentDto>.Fail("Appointment not found"));

            return Ok(ApiResponse<AppointmentDto>.Ok(updated, "Appointment updated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {Id}", id);
            return StatusCode(500, ApiResponse<AppointmentDto>.Fail("Unexpected error"));
        }
    }

    /// <summary>
    /// Cancel an appointment (soft delete).
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var deleted = await _business.Delete(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail("Appointment not found"));

            return Ok(ApiResponse<object>.Ok(null, "Appointment cancelled"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment {Id}", id);
            return StatusCode(500, ApiResponse<object>.Fail("Unexpected error"));
        }
    }

    // Validación
    private static string? ValidateInput(int patientId, int doctorId, DateTime appointmentDateTime)
    {
        if (patientId <= 0)
            return "Validation failed: PatientId must be greater than 0";

        if (doctorId <= 0)
            return "Validation failed: DoctorId must be greater than 0";

        if (appointmentDateTime == default)
            return "Validation failed: AppointmentDateTime is required";

        if (appointmentDateTime < DateTime.Now)
            return "Validation failed: AppointmentDateTime cannot be in the past";

        return null;
    }
}
