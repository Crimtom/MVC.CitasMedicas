using System;
using System.Collections.Generic;

namespace Entities;

public partial class Citum
{
    public int CitaId { get; set; }

    public int PacienteId { get; set; }

    public int DoctorId { get; set; }

    public DateOnly FechaCita { get; set; }

    public TimeOnly HoraCita { get; set; }

    public string? Motivo { get; set; }

    public byte Estado { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Paciente Paciente { get; set; } = null!;
}
