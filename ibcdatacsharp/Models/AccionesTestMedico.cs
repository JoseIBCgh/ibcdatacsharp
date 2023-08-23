using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class AccionesTestMedico
{
    public int NumTest { get; set; }

    public int IdPaciente { get; set; }

    public int IdMedico { get; set; }

    public bool Visto { get; set; }

    public string? Diagnostico { get; set; }

    public virtual User IdMedicoNavigation { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
