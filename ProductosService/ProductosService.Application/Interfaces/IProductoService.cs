using ProductosService.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductosService.Application.Interfaces
{
    public interface IProductoService
    {
        Task<int> CrearProducto(ProductoNuevoDto producto);
        Task<ProductoDto> ObtenerProductoPorId(int idProducto);
        Task<List<ProductoDto>> ObtenerProductos();
    }
}
