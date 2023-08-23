using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class Model
{
    public int Id { get; set; }

    public virtual ICollection<Boundary> Boundaries { get; set; } = new List<Boundary>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<TrainingPoint> TrainingPoints { get; set; } = new List<TrainingPoint>();
}
