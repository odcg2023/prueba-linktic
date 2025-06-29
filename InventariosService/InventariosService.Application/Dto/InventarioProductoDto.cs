using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosService.Application.Dto
{
    public class InventarioProductoDto
    {
        public int IdProducto { get; set; }

        public int ExistenciasActuales { get; set; } = 0;
    }
}
