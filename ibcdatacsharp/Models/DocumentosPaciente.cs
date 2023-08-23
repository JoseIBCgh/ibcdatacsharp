using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class DocumentosPaciente
{
    public int IdPaciente { get; set; }

    public int IdMedico { get; set; }

    public int IdFile { get; set; }

    public virtual File IdFileNavigation { get; set; } = null!;

    public virtual User IdMedicoNavigation { get; set; } = null!;

    public virtual User IdPacienteNavigation { get; set; } = null!;
}
