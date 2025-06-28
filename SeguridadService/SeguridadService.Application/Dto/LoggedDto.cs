using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeguridadService.Application.Dto
{
    /// <summary>
    /// DTO que contiene el token JWT generado tras un inicio de sesión exitoso.
    /// </summary>
    public class LoggedDto
    {
        /// <summary>
        /// Token JWT que puede ser utilizado para autenticar peticiones a la API.
        /// </summary>
        public string Token { get; set; }
    }
}
