using Microsoft.AspNetCore.Mvc;
using SeguridadService.Application.Dto;
using SeguridadService.Application.Interfaces;
using SeguridadService.Service.Helpers;
using static SeguridadService.Common.AppConstants;

namespace SeguridadService.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly JwtHelper _jwtHelper;
        public LoginController(ILoginService loginService, JwtHelper jwtHelper)
        {
            _loginService = loginService;
            _jwtHelper = jwtHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var resultadoLogin = await _loginService.Login(request);

            var tokenGenerado = _jwtHelper.GenerarToken(resultadoLogin);

            return JsonApiResponseFactory.Success(tokenGenerado, "login", Messages.LoginOk);
        }
    }
}
