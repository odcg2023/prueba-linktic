using System;
using System.Collections.Generic;

namespace ProductosService.Domain.Entity;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string NombreProducto { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public bool? Activo { get; set; }

    public short UsuarioCreacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public short? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }
}
