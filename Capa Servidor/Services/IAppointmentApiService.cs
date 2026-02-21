using DTO.Appointment;

namespace API.CitasMedicas.Services;

public interface IAppointmentApiService
{
    Task<List<AppointmentDto>> GetAllAsync(DateTime? date = null);
    Task<AppointmentDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, AppointmentDto? Data)> CreateAsync(AppointmentCreateDto dto);
    Task<(bool Success, string Message, AppointmentDto? Data)> UpdateAsync(int id, AppointmentUpdateDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}
