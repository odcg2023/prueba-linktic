using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Dto
{
    public class CompraDto
    {
        public int IdCompra { get; set; }

        public int IdCliente { get; set; }

        public DateTime FechaCompra { get; set; }

        public int TotalItems { get; set; }
        public decimal ValorTotalCompra { get; set; }
        public List<CompraDetalleDto> Productos { get; set; } = new();

    }
}
