using InventariosService.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosService.Application.Interfaces
{
    public interface IInventarioService
    {
        Task<InventarioProductoDto> ObtenerInventarioProducto(int idProducto);
        Task<InventarioProductoDto> ActualizarInventarioProducto(ActualizaInventarioDto inventario);
        Task<InventarioCompraActualizadoDto> ActualizarInventarioPorCompras(ProductosCompraDto productos);
    }
}
