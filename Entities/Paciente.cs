using System;
using System.Collections.Generic;

namespace Entities;

public partial class Paciente
{
    public int PacienteId { get; set; }

    public string Nombre { get; set; } = null!;

    public int Edad { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<Citum> Cita { get; set; } = new List<Citum>();
}
