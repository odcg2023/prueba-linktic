using System;
using System.Collections.Generic;
using ComprasService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ComprasService.Infraestructure.Context;

public partial class ContextCompras : DbContext
{
    public ContextCompras()
    {
    }

    public ContextCompras(DbContextOptions<ContextCompras> options)
        : base(options)
    {
    }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<CompraDetalle> CompraDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__Compra__0A5CDB5CC25F3F5C");

            entity.ToTable("Compra", "Compras");

            entity.Property(e => e.FechaCompra)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
        });

        modelBuilder.Entity<CompraDetalle>(entity =>
        {
            entity.HasKey(e => e.IdCompraDetalle).HasName("PK__CompraDe__A1B840C5539F5EF8");

            entity.ToTable("CompraDetalle", "Compras");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

            entity.HasOne(d => d.IdCompraNavigation).WithMany(p => p.CompraDetalles)
                .HasForeignKey(d => d.IdCompra)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompraDetalle_Compra");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
