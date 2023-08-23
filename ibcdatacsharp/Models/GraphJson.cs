using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class GraphJson
{
    public int NumTest { get; set; }

    public int IdPaciente { get; set; }

    public string? Graph { get; set; }

    public virtual Test Test { get; set; } = null!;
}
