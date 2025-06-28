using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Dto
{
    public class RegistraCompraDto
    {
        public int IdCliente { get; set; }
        public List<RegistrarCompraDetalleDto> Productos { get; set; }
    }
}
