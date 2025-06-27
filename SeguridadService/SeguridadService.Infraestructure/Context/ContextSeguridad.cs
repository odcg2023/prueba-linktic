using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SeguridadService.Domain.Entity;

namespace SeguridadService.Infraestructure.Context;

public partial class ContextSeguridad : DbContext
{
    public ContextSeguridad()
    {
    }

    public ContextSeguridad(DbContextOptions<ContextSeguridad> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF9735B4F608");

            entity.ToTable("Usuario", "Usuarios");

            entity.HasIndex(e => e.Login, "UQ__Usuario__5E55825B3922DE88").IsUnique();

            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
