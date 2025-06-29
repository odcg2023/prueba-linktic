using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Dto.Integrations
{
    public class InventarioProductoDto
    {
        public int IdProducto { get; set; }

        public int Cantidad { get; set; } = 0;
    }
}
