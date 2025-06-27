using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProductosService.Domain.Entity;

namespace ProductosService.Infraestructure.Context;

public partial class ContextProductos : DbContext
{
    public ContextProductos()
    {
    }

    public ContextProductos(DbContextOptions<ContextProductos> options)
        : base(options)
    {
    }

    public virtual DbSet<Producto> Productos { get; set; }
       

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__098892105024FF88");

            entity.ToTable("Producto", "Productos");

            entity.Property(e => e.Activo)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.NombreProducto).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
