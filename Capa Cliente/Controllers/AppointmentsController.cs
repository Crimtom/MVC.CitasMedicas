using Microsoft.AspNetCore.Mvc;
using MVC.CitasMedicas.Services;
using DTO.Appointment;

namespace MVC.CitasMedicas.Controllers;

public class AppointmentsController : Controller
{
    private readonly IAppointmentService _service;

    public AppointmentsController(IAppointmentService service)
    {
        _service = service;
    }

    // GET: /Appointments
    public async Task<IActionResult> Index()
    {
        var appointments = await _service.GetAllAsync();
        return View(appointments);
    }

    // GET: /Appointments/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Appointments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentCreateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var success = await _service.CreateAsync(dto);
        if (success)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError("", "Error al crear la cita");
        return View(dto);
    }

    // GET: /Appointments/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var appointment = await _service.GetByIdAsync(id);
        if (appointment == null)
            return NotFound();

        var dto = new AppointmentUpdateDto
        {
            PatientId = appointment.PatientId,
            DoctorId = appointment.DoctorId,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Notes = appointment.Notes
        };

        ViewBag.AppointmentId = id;
        return View(dto);
    }

    // POST: /Appointments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AppointmentUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AppointmentId = id;
            return View(dto);
        }

        var success = await _service.UpdateAsync(id, dto);
        if (success)
            return RedirectToAction(nameof(Index));

        ModelState.AddModelError("", "Error al actualizar la cita");
        ViewBag.AppointmentId = id;
        return View(dto);
    }

    // GET: /Appointments/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var appointment = await _service.GetByIdAsync(id);
        if (appointment == null)
            return NotFound();

        return View(appointment);
    }

    // POST: /Appointments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
