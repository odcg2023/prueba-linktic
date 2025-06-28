using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Dto
{
    public class CompraDetalleDto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = null!;
        public int CantidadProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Total
        {
            get
            {
                var cantidad = Math.Max(0, CantidadProducto);
                var precio = Math.Max(0, PrecioUnitario);
                var total = cantidad * precio;
                return Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }
        }
    }
}
