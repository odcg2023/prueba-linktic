using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Dto.Integrations
{
    /// <summary>
    /// DTO para representar un producto consultado.
    /// </summary>
    public class ProductoDto
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public int IdProducto { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string NombreProducto { get; set; } = null!;

        /// <summary>
        /// Descripción del producto.
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Precio del producto.
        /// </summary>
        public decimal Precio { get; set; }

        /// <summary>
        /// Estado del producto (activo o inactivo).
        /// </summary>
        public bool Activo { get; set; }
    }
}
