using System;
using System.Collections.Generic;

namespace ComprasService.Domain.Entity;

public partial class Compra
{
    public int IdCompra { get; set; }

    public int IdCliente { get; set; }

    public DateTime FechaCompra { get; set; }

    public int TotalItems { get; set; }

    public short UsuarioCreacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public short? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();
}
