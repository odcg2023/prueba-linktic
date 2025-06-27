using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeguridadService.Application.Dto
{
    public class LoginRequestDto
    {
        public string UsuarioLogin { get; set; }
        public string Password { get; set; }
    }
}
