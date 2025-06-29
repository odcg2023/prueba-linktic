using ComprasService.Application.Dto.Integrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Interfaces.Integrations
{
    public interface IInventarioApiService
    {
        Task<InventarioProductoDto> ObtenerInventarioProducto(int idProducto, string jwt);
        Task<InventarioCompraActualizadoDto> ActualizarInventario(ProductosCompraDto compra, string jwt);
    }
}
