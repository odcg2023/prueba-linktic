using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SeguridadService.Application.Dto;
using SeguridadService.Application.Interfaces;

namespace SeguridadService.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            var resultadoLogin = _loginService.Login(request);
            return Ok(resultadoLogin);
        }
    }
}
