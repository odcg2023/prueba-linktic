using System;
using System.Collections.Generic;

namespace ComprasService.Domain.Entity;

public partial class CompraDetalle
{
    public int IdCompraDetalle { get; set; }

    public int IdCompra { get; set; }

    public int IdProducto { get; set; }

    public int CantidadProducto { get; set; }

    public short UsuarioCreacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public short? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual Compra IdCompraNavigation { get; set; } = null!;
}
