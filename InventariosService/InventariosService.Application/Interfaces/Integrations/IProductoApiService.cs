using InventariosService.Application.Dto.Integrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosService.Application.Interfaces.Integrations
{
    public interface IProductoApiService
    {
        Task<ProductoDto> ObtenerProductoPorIdAsync(int idProducto, string jwtToken);
    }
}
