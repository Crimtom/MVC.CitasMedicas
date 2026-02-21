using DTO.Appointment;

namespace MVC.CitasMedicas.Services;

public interface IAppointmentService
{
    Task<List<AppointmentDto>> GetAllAsync();
    Task<AppointmentDto?> GetByIdAsync(int id);
    Task<bool> CreateAsync(AppointmentCreateDto dto);
    Task<bool> UpdateAsync(int id, AppointmentUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
