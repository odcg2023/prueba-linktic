using System;
using System.Collections.Generic;
using InventariosService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace InventariosService.Infraestructure.Context;

public partial class ContextInventarios : DbContext
{
    public ContextInventarios()
    {
    }

    public ContextInventarios(DbContextOptions<ContextInventarios> options)
        : base(options)
    {
    }

    public virtual DbSet<Inventario> Inventarios { get; set; }

    public virtual DbSet<InventarioMovimiento> InventarioMovimientos { get; set; }

    public virtual DbSet<InventarioMovimientoDetalle> InventarioMovimientoDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario).HasName("PK__Inventar__1927B20C96A27E47");

            entity.ToTable("Inventario", "Inventarios");

            entity.HasIndex(e => e.IdProducto, "UQ__Inventar__09889211C5722833").IsUnique();

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
        });

        modelBuilder.Entity<InventarioMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PK__Inventar__881A6AE0B60EB2A5");

            entity.ToTable("InventarioMovimiento", "Inventarios");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<InventarioMovimientoDetalle>(entity =>
        {
            entity.HasKey(e => e.IdMovimientoDetalle).HasName("PK__Inventar__9177070209E10ABA");

            entity.ToTable("InventarioMovimientoDetalle", "Inventarios");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdInventarioNavigation).WithMany(p => p.InventarioMovimientoDetalles)
                .HasForeignKey(d => d.IdInventario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventarioMovimientoDetalle_Inventario");

            entity.HasOne(d => d.IdMovimientoNavigation).WithMany(p => p.InventarioMovimientoDetalles)
                .HasForeignKey(d => d.IdMovimiento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventarioMovimientoDetalle_InventarioMovimiento");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
