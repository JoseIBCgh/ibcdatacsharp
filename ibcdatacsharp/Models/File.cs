using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class File
{
    public int Id { get; set; }

    public string? Filename { get; set; }

    public byte[]? Data { get; set; }

    public virtual ICollection<DocumentosPaciente> DocumentosPacientes { get; set; } = new List<DocumentosPaciente>();
}
