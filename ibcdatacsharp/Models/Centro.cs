using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Models;

public partial class Centro
{
    public int Id { get; set; }

    public string? Cif { get; set; }

    public string? NombreFiscal { get; set; }

    public string? Direccion { get; set; }

    public int? Cp { get; set; }

    public string? Ciudad { get; set; }

    public string? Provincia { get; set; }

    public string? Pais { get; set; }

    public int? IdAdmin { get; set; }

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
