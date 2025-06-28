using ComprasService.Application.Dto;
using ComprasService.Application.Dto.JsonResponse;
using ComprasService.Application.Interfaces;
using ComprasService.Service.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ComprasService.Common.AppConstants;

namespace ComprasService.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public ComprasController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        public async Task<IActionResult> RegistrarCompra(RegistraCompraDto compra)
        {
            var registroCompra = await _compraService.RegistrarCompra(compra);
            return JsonApiResponseFactory.Success(registroCompra, "compra", Messages.CompraRegistrada);
        }
    }
}
