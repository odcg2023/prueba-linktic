using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Dto
{
    public class CompraDetalleDto
    {
        public int IdCompraDetalle { get; set; }

        public int IdCompra { get; set; }

        public int IdProducto { get; set; }

        public int CantidadProducto { get; set; }
    }
}
