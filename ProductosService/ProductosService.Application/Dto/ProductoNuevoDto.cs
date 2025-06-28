using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductosService.Application.Dto
{
    /// <summary>
    /// DTO para crear un nuevo producto.
    /// </summary>
    public class ProductoNuevoDto
    {
        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string NombreProducto { get; set; } = null!;

        /// <summary>
        /// Descripción opcional del producto.
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Precio del producto.
        /// </summary>
        public decimal Precio { get; set; }
    }
}
