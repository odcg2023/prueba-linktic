using ComprasService.Application.Dto;
using ComprasService.Application.Dto.JsonResponse;
using ComprasService.Application.Interfaces;
using ComprasService.Service.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ComprasService.Common.AppConstants;

namespace ComprasService.Service.Controllers
{
    [Authorize]
    /// <summary>
    /// API para gestionar las compras.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ComprasController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public ComprasController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        /// <summary>
        /// Registra una nueva compra en el sistema.
        /// </summary>
        /// <param name="compra">Datos de la compra a registrar.</param>
        /// <returns>La compra registrada con su Id y detalles.</returns>
        /// <response code="200">Compra registrada exitosamente.</response>
        [HttpPost]
        [Route("registrar-compra")]
        [ProducesResponseType(typeof(CompraDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegistrarCompra([FromBody] RegistraCompraDto compra)
        {
            var registroCompra = await _compraService.RegistrarCompra(compra);
            return JsonApiResponseFactory.Success(registroCompra, "compra", Messages.CompraRegistrada);
        }

        /// <summary>
        /// Obtiene todas las compras registradas en el sistema.
        /// </summary>
        /// <returns>Lista de compras.</returns>
        /// <response code="200">Compras obtenidas exitosamente.</response>
        [HttpGet]
        [Route("obtener-compras")]
        [ProducesResponseType(typeof(List<CompraDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerCompras()
        {
            var compras = await _compraService.ObtenerCompras();
            return JsonApiResponseFactory.Success(compras, "compra", "Compras obtenidas correctamente");
        }

        /// <summary>
        /// Obtiene una compra específica por su identificador.
        /// </summary>
        /// <param name="id">Id de la compra.</param>
        /// <returns>Datos de la compra encontrada.</returns>
        /// <response code="200">Compra obtenida exitosamente.</response>
        /// <response code="404">No se encontró la compra con el Id proporcionado.</response>
        [HttpGet("obtener-compra-por-id/{id}")]
        [ProducesResponseType(typeof(CompraDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerCompraPorId(int id)
        {
            var compra = await _compraService.ObtenerCompraPorId(id);
            return JsonApiResponseFactory.Success(compra, "compra", $"Compra {id} obtenida correctamente");
        }
    }

}
