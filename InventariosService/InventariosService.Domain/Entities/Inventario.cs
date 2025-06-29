using System;
using System.Collections.Generic;

namespace InventariosService.Domain.Entity;

public partial class Inventario
{
    public int IdInventario { get; set; }

    public int IdProducto { get; set; }

    public int ExistenciasActuales { get; set; }

    public short UsuarioCreacion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public short? UsuarioModificacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<InventarioMovimientoDetalle> InventarioMovimientoDetalles { get; set; } = new List<InventarioMovimientoDetalle>();
}
