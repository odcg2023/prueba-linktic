using System;
using System.Collections.Generic;

namespace InventariosService.Domain.Entity;

public partial class InventarioMovimiento
{
    public int IdMovimiento { get; set; }

    public byte TipoMovimiento { get; set; }

    public string? Observaciones { get; set; }

    public short UsuarioCreacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<InventarioMovimientoDetalle> InventarioMovimientoDetalles { get; set; } = new List<InventarioMovimientoDetalle>();
}
