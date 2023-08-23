using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class Boundary
{
    public int ModelId { get; set; }

    public int Index { get; set; }

    public float? Intercept { get; set; }

    public float? Coef0 { get; set; }

    public float? Coef1 { get; set; }

    public float? Coef2 { get; set; }

    public virtual Model Model { get; set; } = null!;
}
