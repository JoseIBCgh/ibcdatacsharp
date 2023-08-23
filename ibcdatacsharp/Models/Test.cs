using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class Test
{
    public int NumTest { get; set; }

    public int IdPaciente { get; set; }

    public int? IdCentro { get; set; }

    public DateOnly? Date { get; set; }

    public float? Bow { get; set; }

    public float? FallToLeft { get; set; }

    public float? FallToRight { get; set; }

    public float? FallingBackward { get; set; }

    public float? FallingForward { get; set; }

    public float? Idle { get; set; }

    public float? Sitting { get; set; }

    public float? Sleep { get; set; }

    public float? Standing { get; set; }

    public int? Model { get; set; }

    public virtual ICollection<AccionesTestMedico> AccionesTestMedicos { get; set; } = new List<AccionesTestMedico>();

    public virtual GraphJson? GraphJson { get; set; }

    public virtual Centro? IdCentroNavigation { get; set; }

    public virtual User IdPacienteNavigation { get; set; } = null!;

    public virtual Model? ModelNavigation { get; set; }

    public virtual ICollection<TestUnit> TestUnits { get; set; } = new List<TestUnit>();
}
