using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class TrainingPoint
{
    public int ModelId { get; set; }

    public int Index { get; set; }

    public string? Clase { get; set; }

    public float? AccX { get; set; }

    public float? AccY { get; set; }

    public float? AccZ { get; set; }

    public virtual Model Model { get; set; } = null!;
}
