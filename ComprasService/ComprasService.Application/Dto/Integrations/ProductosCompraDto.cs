
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Dto.Integrations
{
    public class ProductosCompraDto
    {
        public byte TipoMovimiento { get; set; }
        public List<InventarioProductoDto> ListaProductos { get; set; }
        public string Observaciones { get; set; } 
    }
}
