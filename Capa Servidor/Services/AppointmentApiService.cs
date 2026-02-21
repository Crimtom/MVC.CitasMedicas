using System.Net;
using System.Net.Http.Json;
using DTO.Appointment;
using DTO.Common;

namespace API.CitasMedicas.Services;

public class AppointmentApiService : IAppointmentApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AppointmentApiService> _logger;

    public AppointmentApiService(HttpClient httpClient, ILogger<AppointmentApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<AppointmentDto>> GetAllAsync(DateTime? date = null)
    {
        try
        {
            var url = date.HasValue 
                ? $"api/citas?date={date.Value:yyyy-MM-dd}" 
                : "api/citas";
            
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AppointmentDto>>>(url);
            return response?.Data ?? new List<AppointmentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments from API");
            return new List<AppointmentDto>();
        }
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<AppointmentDto>>($"api/citas/{id}");
            return response?.Data;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointment {Id} from API", id);
            return null;
        }
    }

    public async Task<(bool Success, string Message, AppointmentDto? Data)> CreateAsync(AppointmentCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/citas", dto);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AppointmentDto>>();
            
            if (response.IsSuccessStatusCode)
                return (true, result?.Message ?? "Created", result?.Data);
            
            return (false, result?.Message ?? "Error creating appointment", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating appointment");
            return (false, "Unexpected error", null);
        }
    }

    public async Task<(bool Success, string Message, AppointmentDto? Data)> UpdateAsync(int id, AppointmentUpdateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/citas/{id}", dto);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AppointmentDto>>();
            
            if (response.IsSuccessStatusCode)
                return (true, result?.Message ?? "Updated", result?.Data);
            
            return (false, result?.Message ?? "Error updating appointment", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating appointment {Id}", id);
            return (false, "Unexpected error", null);
        }
    }

    public async Task<(bool Success, string Message)> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/citas/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            
            if (response.IsSuccessStatusCode)
                return (true, result?.Message ?? "Deleted");
            
            return (false, result?.Message ?? "Error deleting appointment");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment {Id}", id);
            return (false, "Unexpected error");
        }
    }
}
