using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeguridadService.Application.Dto
{
    /// <summary>
    /// DTO para la solicitud de login.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Nombre de usuario o login que intenta autenticarse.
        /// </summary>
        public string UsuarioLogin { get; set; }

        /// <summary>
        /// Contraseña cifrada del usuario.
        /// </summary>
        public string Password { get; set; }
    }
}
