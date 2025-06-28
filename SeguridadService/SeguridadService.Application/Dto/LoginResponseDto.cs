using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeguridadService.Application.Dto
{
    /// <summary>
    /// DTO intermedio con los datos básicos del usuario tras autenticarse.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Identificador único del usuario autenticado.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del usuario autenticado.
        /// </summary>
        public string NombreUsuario { get; set; }
    }
}
