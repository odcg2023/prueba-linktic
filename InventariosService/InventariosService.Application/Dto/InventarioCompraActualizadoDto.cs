using InventariosService.Application.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosService.Application.Dto
{
    public class InventarioCompraActualizadoDto
    {
        public int IdMovimiento { get; set; }

        public TipoMovimiento TipoMovimiento { get; set; }
    }
}
