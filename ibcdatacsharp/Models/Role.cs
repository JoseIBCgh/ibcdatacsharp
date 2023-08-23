using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Permissions { get; set; }

    public DateTime UpdateDatetime { get; set; }
}
