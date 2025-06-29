using InventariosService.Application.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosService.Application.Dto
{
    public class ProductosCompraDto
    {
        public TipoMovimiento TipoMovimiento { get; set; }
        public List<ProductoInventarioDto> ListaProductos { get; set; }
        public string Observaciones { get; set; } 
    }
}
