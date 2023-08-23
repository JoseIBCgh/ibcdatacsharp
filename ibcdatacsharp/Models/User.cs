using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ibcdatacsharp.Models;

public partial class User
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool Active { get; set; }

    public string FsUniquifier { get; set; } = null!;

    public DateTime? ConfirmedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? CurrentLoginAt { get; set; }

    public string? LastLoginIp { get; set; }

    public string? CurrentLoginIp { get; set; }

    public int? LoginCount { get; set; }

    public string? TfPrimaryMethod { get; set; }

    public string? TfTotpSecret { get; set; }

    public string? TfPhoneNumber { get; set; }

    public DateTime CreateDatetime { get; set; }

    public DateTime UpdateDatetime { get; set; }

    public string? UsTotpSecrets { get; set; }

    public string? UsPhoneNumber { get; set; }

    public int Id { get; set; }

    public string? Identificador { get; set; }

    public string? Username { get; set; }

    public string? Nombre { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? Sexo { get; set; }

    public float? Altura { get; set; }

    public float? Peso { get; set; }

    public string? AntecedentesClinicos { get; set; }

    public int? IdCentro { get; set; }

    public virtual ICollection<AccionesTestMedico> AccionesTestMedicos { get; set; } = new List<AccionesTestMedico>();

    public virtual ICollection<DocumentosPaciente> DocumentosPacienteIdMedicoNavigations { get; set; } = new List<DocumentosPaciente>();

    public virtual ICollection<DocumentosPaciente> DocumentosPacienteIdPacienteNavigations { get; set; } = new List<DocumentosPaciente>();

    public virtual Centro? IdCentroNavigation { get; set; }

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<User> IdMedicos { get; set; } = new List<User>();

    public virtual ICollection<User> IdPacientes { get; set; } = new List<User>();
}
