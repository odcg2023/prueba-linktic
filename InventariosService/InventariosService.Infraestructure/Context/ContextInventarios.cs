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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario).HasName("PK__Inventar__1927B20CBBCBDB1D");

            entity.ToTable("Inventario", "Inventarios");

            entity.HasIndex(e => e.IdProducto, "UQ__Inventar__09889211E85E1A65").IsUnique();

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
        });

        modelBuilder.Entity<InventarioMovimiento>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PK__Inventar__881A6AE01383488A");

            entity.ToTable("InventarioMovimiento", "Inventarios");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdInventarioNavigation).WithMany(p => p.InventarioMovimientos)
                .HasForeignKey(d => d.IdInventario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventarioMovimiento_Inventario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
