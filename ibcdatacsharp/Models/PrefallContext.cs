using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ibcdatacsharp.Models;

public partial class PrefallContext : DbContext
{
    public PrefallContext()
    {
    }

    public PrefallContext(DbContextOptions<PrefallContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccionesTestMedico> AccionesTestMedicos { get; set; }

    public virtual DbSet<Boundary> Boundaries { get; set; }

    public virtual DbSet<Centro> Centros { get; set; }

    public virtual DbSet<DocumentosPaciente> DocumentosPacientes { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<GraphJson> GraphJsons { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolesUser> RolesUsers { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestUnit> TestUnits { get; set; }

    public virtual DbSet<TrainingPoint> TrainingPoints { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=prefall;user=webapp;password=webapp", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.34-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<AccionesTestMedico>(entity =>
        {
            entity.HasKey(e => new { e.NumTest, e.IdPaciente, e.IdMedico })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("acciones_test_medico");

            entity.HasIndex(e => e.IdMedico, "id_medico");

            entity.Property(e => e.NumTest).HasColumnName("num_test");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.Diagnostico)
                .HasColumnType("text")
                .HasColumnName("diagnostico");
            entity.Property(e => e.Visto).HasColumnName("visto");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.AccionesTestMedicos)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("acciones_test_medico_ibfk_2");

            entity.HasOne(d => d.Test).WithMany(p => p.AccionesTestMedicos)
                .HasForeignKey(d => new { d.NumTest, d.IdPaciente })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("acciones_test_medico_ibfk_1");
        });

        modelBuilder.Entity<Boundary>(entity =>
        {
            entity.HasKey(e => new { e.ModelId, e.Index })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("boundary");

            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.Index).HasColumnName("index");
            entity.Property(e => e.Coef0).HasColumnName("coef0");
            entity.Property(e => e.Coef1).HasColumnName("coef1");
            entity.Property(e => e.Coef2).HasColumnName("coef2");
            entity.Property(e => e.Intercept).HasColumnName("intercept");

            entity.HasOne(d => d.Model).WithMany(p => p.Boundaries)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("boundary_ibfk_1");
        });

        modelBuilder.Entity<Centro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("centros");

            entity.HasIndex(e => e.Cif, "cif").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cif)
                .HasMaxLength(20)
                .HasColumnName("cif");
            entity.Property(e => e.Ciudad)
                .HasMaxLength(30)
                .HasColumnName("ciudad");
            entity.Property(e => e.Cp).HasColumnName("CP");
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .HasColumnName("direccion");
            entity.Property(e => e.IdAdmin).HasColumnName("id_admin");
            entity.Property(e => e.NombreFiscal)
                .HasMaxLength(50)
                .HasColumnName("nombreFiscal");
            entity.Property(e => e.Pais)
                .HasMaxLength(20)
                .HasColumnName("pais");
            entity.Property(e => e.Provincia)
                .HasMaxLength(30)
                .HasColumnName("provincia");
        });

        modelBuilder.Entity<DocumentosPaciente>(entity =>
        {
            entity.HasKey(e => new { e.IdPaciente, e.IdMedico, e.IdFile })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("documentos_paciente");

            entity.HasIndex(e => e.IdFile, "id_file");

            entity.HasIndex(e => e.IdMedico, "id_medico");

            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.IdMedico).HasColumnName("id_medico");
            entity.Property(e => e.IdFile).HasColumnName("id_file");

            entity.HasOne(d => d.IdFileNavigation).WithMany(p => p.DocumentosPacientes)
                .HasForeignKey(d => d.IdFile)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("documentos_paciente_ibfk_3");

            entity.HasOne(d => d.IdMedicoNavigation).WithMany(p => p.DocumentosPacienteIdMedicoNavigations)
                .HasForeignKey(d => d.IdMedico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("documentos_paciente_ibfk_2");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.DocumentosPacienteIdPacienteNavigations)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("documentos_paciente_ibfk_1");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("files");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data)
                .HasColumnType("blob")
                .HasColumnName("data");
            entity.Property(e => e.Filename)
                .HasMaxLength(50)
                .HasColumnName("filename");
        });

        modelBuilder.Entity<GraphJson>(entity =>
        {
            entity.HasKey(e => new { e.NumTest, e.IdPaciente })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("graph_json");

            entity.Property(e => e.NumTest).HasColumnName("num_test");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.Graph)
                .HasColumnType("json")
                .HasColumnName("graph");

            entity.HasOne(d => d.Test).WithOne(p => p.GraphJson)
                .HasForeignKey<GraphJson>(d => new { d.NumTest, d.IdPaciente })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("graph_json_ibfk_1");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("model");

            entity.Property(e => e.Id).HasColumnName("id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
            entity.Property(e => e.Permissions)
                .HasColumnType("text")
                .HasColumnName("permissions");
            entity.Property(e => e.UpdateDatetime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("update_datetime");
        });

        modelBuilder.Entity<RolesUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("roles_users");

            entity.HasIndex(e => e.RoleId, "role_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Role).WithMany()
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("roles_users_ibfk_2");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("roles_users_ibfk_1");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => new { e.NumTest, e.IdPaciente })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("test");

            entity.HasIndex(e => e.IdCentro, "id_centro");

            entity.HasIndex(e => e.IdPaciente, "id_paciente");

            entity.HasIndex(e => e.Model, "model");

            entity.Property(e => e.NumTest).HasColumnName("num_test");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.Bow).HasColumnName("bow");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.FallToLeft).HasColumnName("fall_to_left");
            entity.Property(e => e.FallToRight).HasColumnName("fall_to_right");
            entity.Property(e => e.FallingBackward).HasColumnName("falling_backward");
            entity.Property(e => e.FallingForward).HasColumnName("falling_forward");
            entity.Property(e => e.IdCentro).HasColumnName("id_centro");
            entity.Property(e => e.Idle).HasColumnName("idle");
            entity.Property(e => e.Model).HasColumnName("model");
            entity.Property(e => e.Sitting).HasColumnName("sitting");
            entity.Property(e => e.Sleep).HasColumnName("sleep");
            entity.Property(e => e.Standing).HasColumnName("standing");

            entity.HasOne(d => d.IdCentroNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.IdCentro)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("test_ibfk_2");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("test_ibfk_1");

            entity.HasOne(d => d.ModelNavigation).WithMany(p => p.Tests)
                .HasForeignKey(d => d.Model)
                .HasConstraintName("test_ibfk_3");
        });

        modelBuilder.Entity<TestUnit>(entity =>
        {
            entity.HasKey(e => new { e.NumTest, e.IdPaciente, e.Time })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("test_unit");

            entity.Property(e => e.NumTest).HasColumnName("num_test");
            entity.Property(e => e.IdPaciente).HasColumnName("id_paciente");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.AccX).HasColumnName("acc_x");
            entity.Property(e => e.AccY).HasColumnName("acc_y");
            entity.Property(e => e.AccZ).HasColumnName("acc_z");
            entity.Property(e => e.GyrX).HasColumnName("gyr_x");
            entity.Property(e => e.GyrY).HasColumnName("gyr_y");
            entity.Property(e => e.GyrZ).HasColumnName("gyr_z");
            entity.Property(e => e.Item).HasColumnName("item");
            entity.Property(e => e.MagX).HasColumnName("mag_x");
            entity.Property(e => e.MagY).HasColumnName("mag_y");
            entity.Property(e => e.MagZ).HasColumnName("mag_z");

            entity.HasOne(d => d.Test).WithMany(p => p.TestUnits)
                .HasForeignKey(d => new { d.NumTest, d.IdPaciente })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("test_unit_ibfk_1");
        });

        modelBuilder.Entity<TrainingPoint>(entity =>
        {
            entity.HasKey(e => new { e.ModelId, e.Index })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("training_point");

            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.Index).HasColumnName("index");
            entity.Property(e => e.AccX).HasColumnName("acc_x");
            entity.Property(e => e.AccY).HasColumnName("acc_y");
            entity.Property(e => e.AccZ).HasColumnName("acc_z");
            entity.Property(e => e.Clase)
                .HasMaxLength(20)
                .HasColumnName("clase");

            entity.HasOne(d => d.Model).WithMany(p => p.TrainingPoints)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("training_point_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.FsUniquifier, "fs_uniquifier").IsUnique();

            entity.HasIndex(e => e.IdCentro, "id_centro");

            entity.HasIndex(e => e.Identificador, "identificador").IsUnique();

            entity.HasIndex(e => e.Username, "username").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Altura).HasColumnName("altura");
            entity.Property(e => e.AntecedentesClinicos)
                .HasColumnType("text")
                .HasColumnName("antecedentes_clinicos");
            entity.Property(e => e.ConfirmedAt)
                .HasColumnType("datetime")
                .HasColumnName("confirmed_at");
            entity.Property(e => e.CreateDatetime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("create_datetime");
            entity.Property(e => e.CurrentLoginAt)
                .HasColumnType("datetime")
                .HasColumnName("current_login_at");
            entity.Property(e => e.CurrentLoginIp)
                .HasMaxLength(64)
                .HasColumnName("current_login_ip");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.FsUniquifier)
                .HasMaxLength(64)
                .HasColumnName("fs_uniquifier");
            entity.Property(e => e.IdCentro).HasColumnName("id_centro");
            entity.Property(e => e.Identificador)
                .HasMaxLength(10)
                .HasColumnName("identificador");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("datetime")
                .HasColumnName("last_login_at");
            entity.Property(e => e.LastLoginIp)
                .HasMaxLength(64)
                .HasColumnName("last_login_ip");
            entity.Property(e => e.LoginCount).HasColumnName("login_count");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Peso).HasColumnName("peso");
            entity.Property(e => e.Sexo)
                .HasMaxLength(1)
                .HasColumnName("sexo");
            entity.Property(e => e.TfPhoneNumber)
                .HasMaxLength(128)
                .HasColumnName("tf_phone_number");
            entity.Property(e => e.TfPrimaryMethod)
                .HasMaxLength(64)
                .HasColumnName("tf_primary_method");
            entity.Property(e => e.TfTotpSecret)
                .HasMaxLength(255)
                .HasColumnName("tf_totp_secret");
            entity.Property(e => e.UpdateDatetime)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("update_datetime");
            entity.Property(e => e.UsPhoneNumber)
                .HasMaxLength(128)
                .HasColumnName("us_phone_number");
            entity.Property(e => e.UsTotpSecrets)
                .HasColumnType("text")
                .HasColumnName("us_totp_secrets");
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .HasColumnName("username");

            entity.HasOne(d => d.IdCentroNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdCentro)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("users_ibfk_1");

            entity.HasMany(d => d.IdMedicos).WithMany(p => p.IdPacientes)
                .UsingEntity<Dictionary<string, object>>(
                    "PacientesAsociado",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("IdMedico")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("pacientes_asociados_ibfk_2"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdPaciente")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("pacientes_asociados_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdPaciente", "IdMedico")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("pacientes_asociados");
                        j.HasIndex(new[] { "IdMedico" }, "id_medico");
                        j.IndexerProperty<int>("IdPaciente").HasColumnName("id_paciente");
                        j.IndexerProperty<int>("IdMedico").HasColumnName("id_medico");
                    });

            entity.HasMany(d => d.IdPacientes).WithMany(p => p.IdMedicos)
                .UsingEntity<Dictionary<string, object>>(
                    "PacientesAsociado",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("IdPaciente")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("pacientes_asociados_ibfk_1"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdMedico")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("pacientes_asociados_ibfk_2"),
                    j =>
                    {
                        j.HasKey("IdPaciente", "IdMedico")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("pacientes_asociados");
                        j.HasIndex(new[] { "IdMedico" }, "id_medico");
                        j.IndexerProperty<int>("IdPaciente").HasColumnName("id_paciente");
                        j.IndexerProperty<int>("IdMedico").HasColumnName("id_medico");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
