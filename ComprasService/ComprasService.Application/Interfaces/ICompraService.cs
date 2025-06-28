using ComprasService.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Interfaces
{
    public interface ICompraService
    {
        Task<CompraDto> RegistrarCompra(RegistraCompraDto nuevaCompra);
        Task<List<CompraDto>> ObtenerCompras();
        Task<CompraDto> ObtenerCompraPorId(int idCompra);
    }
}
