using DTO.Appointment;

namespace Business.Interfaces;

public interface IAppointmentBusiness
{
    // Cascaron de la api
    Task<List<AppointmentDto>> GetAll(DateOnly? date);
    Task<AppointmentDto?> GetById(int id);
    Task<AppointmentDto> Create(AppointmentCreateDto dto);
    Task<AppointmentDto?> Update(int id, AppointmentUpdateDto dto);
    Task<bool> Delete(int id);

    // Funciones de ayuda
    Task<bool> PatientExists(int id);
    Task<bool> DoctorExists(int id);
    Task<bool> HasConflict(int doctorId, DateOnly date, TimeOnly time, int? excludeId = null);
}
