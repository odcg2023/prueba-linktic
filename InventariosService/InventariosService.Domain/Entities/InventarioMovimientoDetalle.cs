using System;
using System.Collections.Generic;

namespace InventariosService.Domain.Entity;

public partial class InventarioMovimientoDetalle
{
    public int IdMovimientoDetalle { get; set; }

    public int IdMovimiento { get; set; }

    public int IdInventario { get; set; }

    public int CantidadAntes { get; set; }

    public int CantidadMovimiento { get; set; }

    public int CantidadDespues { get; set; }

    public string? Observaciones { get; set; }

    public short UsuarioCreacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Inventario IdInventarioNavigation { get; set; } = null!;

    public virtual InventarioMovimiento IdMovimientoNavigation { get; set; } = null!;
}
