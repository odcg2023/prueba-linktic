using SeguridadService.Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeguridadService.Application.Interfaces
{
    public interface ILoginService
    {
        LoginResponseDto Login(LoginRequestDto login);
    }
}
