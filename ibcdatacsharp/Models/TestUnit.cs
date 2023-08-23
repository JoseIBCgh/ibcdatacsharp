using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class TestUnit
{
    public int NumTest { get; set; }

    public int IdPaciente { get; set; }

    public int? Item { get; set; }

    public double Time { get; set; }

    public float? AccX { get; set; }

    public float? AccY { get; set; }

    public float? AccZ { get; set; }

    public float? GyrX { get; set; }

    public float? GyrY { get; set; }

    public float? GyrZ { get; set; }

    public float? MagX { get; set; }

    public float? MagY { get; set; }

    public float? MagZ { get; set; }

    public virtual Test Test { get; set; } = null!;
}
