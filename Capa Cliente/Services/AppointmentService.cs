using System.Net.Http.Json;
using DTO.Appointment;
using DTO.Common;

namespace MVC.CitasMedicas.Services;

public class AppointmentService : IAppointmentService
{
    private readonly HttpClient _httpClient;

    public AppointmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AppointmentDto>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<AppointmentDto>>>("api/citas");
            return response?.Data ?? new List<AppointmentDto>();
        }
        catch
        {
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
        catch
        {
            return null;
        }
    }

    public async Task<bool> CreateAsync(AppointmentCreateDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/citas", dto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(int id, AppointmentUpdateDto dto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/citas/{id}", dto);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/citas/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
