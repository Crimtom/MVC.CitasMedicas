using System;
using System.Collections.Generic;

namespace Entities;

public partial class VwCitasDetalle
{
    public int CitaId { get; set; }

    public int PacienteId { get; set; }

    public string Paciente { get; set; } = null!;

    public int DoctorId { get; set; }

    public string Doctor { get; set; } = null!;

    public int EspecialidadId { get; set; }

    public string Especialidad { get; set; } = null!;

    public DateOnly FechaCita { get; set; }

    public TimeOnly HoraCita { get; set; }

    public string? Motivo { get; set; }

    public byte Estado { get; set; }
}
