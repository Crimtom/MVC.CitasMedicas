using Business.Interfaces;
using DataAcces.Models;
using DTO.Appointment;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Business;

public class AppointmentBusiness : IAppointmentBusiness
{
    private readonly CitasMedicasContext _context;

    // Constructor
    public AppointmentBusiness(CitasMedicasContext context)
    {
        _context = context;
    }

    // Métodos de la api

    public async Task<List<AppointmentDto>> GetAll(DateOnly? date)
    {
        // Creando comando sql desde código

        // SELECT PacienteId,DoctorId,FechaCita,HoraCita,Motivo,Estado FROM CITAS WHERE fecha = '2/21/2026';

        var query = _context.Cita
            .Include(c => c.Paciente)
            .Include(c => c.Doctor)
            .Where(c => c.Estado != 3)
            .AsQueryable();

        if (date.HasValue)
            query = query.Where(c => c.FechaCita == date.Value);

        var citas = await query
            .OrderByDescending(c => c.FechaCita)
            .ThenByDescending(c => c.HoraCita)
            .ToListAsync();

        return citas.Select(MapToDto).ToList();
    }

    public async Task<AppointmentDto?> GetById(int id)
    {

        // SELECT PacienteId, DoctorId WHERE CitaId = id;
        var cita = await _context.Cita
            .Include(c => c.Paciente)
            .Include(c => c.Doctor)
            .FirstOrDefaultAsync(c => c.CitaId == id);

        return cita is null ? null : MapToDto(cita); //PacienteId: 1 , ....
    }

    public async Task<AppointmentDto> Create(AppointmentCreateDto dto)
    {
        var cita = new Citum
        {
            PacienteId = dto.PatientId,
            DoctorId = dto.DoctorId,
            FechaCita = DateOnly.FromDateTime(dto.AppointmentDateTime),
            HoraCita = TimeOnly.FromDateTime(dto.AppointmentDateTime),
            Motivo = dto.Notes,
            Estado = 1
        };

        _context.Cita.Add(cita);
        await _context.SaveChangesAsync();

        return (await GetById(cita.CitaId))!;
    }

    public async Task<AppointmentDto?> Update(int id, AppointmentUpdateDto dto)
    {
        var cita = await _context.Cita.FindAsync(id);

        // Validación
        if (cita is null) return null;

        cita.PacienteId = dto.PatientId;
        cita.DoctorId = dto.DoctorId;
        cita.FechaCita = DateOnly.FromDateTime(dto.AppointmentDateTime);
        cita.HoraCita = TimeOnly.FromDateTime(dto.AppointmentDateTime);
        cita.Motivo = dto.Notes;

        await _context.SaveChangesAsync();

        return (await GetById(id))!;
    }

    public async Task<bool> Delete(int id)
    {
        var cita = await _context.Cita.FindAsync(id);

        // Validación
        if (cita is null) return false;

        cita.Estado = 3;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PatientExists(int id)
    {
        return await _context.Pacientes.AnyAsync(p => p.PacienteId == id);
    }

    public async Task<bool> DoctorExists(int id)
    {
        return await _context.Doctors.AnyAsync(d => d.DoctorId == id);
    }

    public async Task<bool> HasConflict(int doctorId, DateOnly date, TimeOnly time, int? excludeId = null)
    {
        var query = _context.Cita
            .Where(c => c.DoctorId == doctorId
                     && c.FechaCita == date
                     && c.HoraCita == time
                     && c.Estado != 3);
        // Validación
        if (excludeId.HasValue)
            query = query.Where(c => c.CitaId != excludeId.Value);

        return await query.AnyAsync();
    }

    private static AppointmentDto MapToDto(Citum cita) => new()
    {
        Id = cita.CitaId,
        PatientId = cita.PacienteId,
        PatientName = cita.Paciente?.Nombre ?? string.Empty,
        DoctorId = cita.DoctorId,
        DoctorName = cita.Doctor?.Nombre ?? string.Empty,
        AppointmentDateTime = cita.FechaCita.ToDateTime(cita.HoraCita),
        Notes = cita.Motivo,
        Status = cita.Estado switch
        {
            1 => "Scheduled",
            2 => "Confirmed",
            3 => "Cancelled",
            _ => "Unknown"
        }
    };
}
