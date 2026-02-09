using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace DataAcces.Models;

public partial class CitasMedicasContext : DbContext
{
    public CitasMedicasContext()
    {
    }

    public CitasMedicasContext(DbContextOptions<CitasMedicasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Citum> Cita { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Especialidad> Especialidads { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<VwCitasDetalle> VwCitasDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Citum>(entity =>
        {
            entity.HasKey(e => e.CitaId);

            entity.HasIndex(e => e.DoctorId, "IX_Cita_DoctorId");

            entity.HasIndex(e => e.FechaCita, "IX_Cita_FechaCita");

            entity.HasIndex(e => e.PacienteId, "IX_Cita_PacienteId");

            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_Cita_Estado");
            entity.Property(e => e.FechaCreacion)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Cita_Fecha");
            entity.Property(e => e.HoraCita).HasPrecision(0);
            entity.Property(e => e.Motivo).HasMaxLength(500);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Cita)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cita_Doctor");

            entity.HasOne(d => d.Paciente).WithMany(p => p.Cita)
                .HasForeignKey(d => d.PacienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cita_Paciente");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctor");

            entity.HasIndex(e => e.EspecialidadId, "IX_Doctor_EspecialidadId");

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Doctor_Activo");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaCreacion)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Doctor_Fecha");
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Telefono).HasMaxLength(30);

            entity.HasOne(d => d.Especialidad).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.EspecialidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctor_Especialidad");
        });

        modelBuilder.Entity<Especialidad>(entity =>
        {
            entity.ToTable("Especialidad");

            entity.HasIndex(e => e.Nombre, "UQ_Especialidad").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Especialidad_Activo");
            entity.Property(e => e.FechaCreacion)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Especialidad_Fecha");
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.ToTable("Paciente");

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Paciente_Activo");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FechaCreacion)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Paciente_Fecha");
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Telefono).HasMaxLength(30);
        });

        modelBuilder.Entity<VwCitasDetalle>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_CitasDetalle");

            entity.Property(e => e.Doctor).HasMaxLength(150);
            entity.Property(e => e.Especialidad).HasMaxLength(100);
            entity.Property(e => e.HoraCita).HasPrecision(0);
            entity.Property(e => e.Motivo).HasMaxLength(500);
            entity.Property(e => e.Paciente).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
