using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductosService.Application.Dto
{
    public class ProductoDto
    {
        public int IdProducto { get; set; }

        public string NombreProducto { get; set; } = null!;

        public string? Descripcion { get; set; }

        public decimal Precio { get; set; }

        public bool? Activo { get; set; }
    }
}
