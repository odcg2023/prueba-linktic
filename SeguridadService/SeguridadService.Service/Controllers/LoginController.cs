using Microsoft.AspNetCore.Mvc;
using SeguridadService.Application.Dto;
using SeguridadService.Application.Interfaces;
using SeguridadService.Service.Helpers;
using static SeguridadService.Common.AppConstants;

namespace SeguridadService.Service.Controllers
{
    /// <summary>
    /// Controlador para autenticación de usuarios en el sistema.
    /// </summary>
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

        /// <summary>
        /// Autentica un usuario con su login y contraseña, devolviendo un token JWT si es exitoso.
        /// </summary>
        /// <param name="request">Objeto con el login y la contraseña cifrada.</param>
        /// <returns>
        /// Retorna un objeto JSON:API con el token JWT si el login es correcto,
        /// o un error JSON:API en caso de credenciales inválidas o usuario inactivo.
        /// </returns>
        /// <response code="200">Autenticación exitosa, retorna el token JWT.</response>
        /// <response code="400">Error de validación o credenciales incorrectas.</response>
        /// <response code="500">Error interno inesperado.</response>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var resultadoLogin = await _loginService.Login(request);

            var tokenGenerado = _jwtHelper.GenerarToken(resultadoLogin);

            return JsonApiResponseFactory.Success(tokenGenerado, "login", Messages.LoginOk);
        }
    }
}
