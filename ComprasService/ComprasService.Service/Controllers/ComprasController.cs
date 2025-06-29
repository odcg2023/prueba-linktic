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
    /// API para gestionar las compras del sistema.
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
        /// <param name="compra">Objeto con los datos de la compra a registrar.</param>
        /// <returns>La compra registrada con su Id y sus detalles.</returns>
        /// <response code="200">Compra registrada exitosamente.</response>
        /// <response code="400">Error en los datos enviados o reglas de negocio no cumplidas.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("registrar-compra")]
        [ProducesResponseType(typeof(JsonApiResponse<CompraDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(JsonApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarCompra([FromBody] RegistraCompraDto compra)
        {
            var registroCompra = await _compraService.RegistrarCompra(compra);
            return JsonApiResponseFactory.Success(registroCompra, "compra", Messages.CompraRegistrada);
        }

        /// <summary>
        /// Obtiene todas las compras registradas en el sistema.
        /// </summary>
        /// <returns>Lista de todas las compras existentes.</returns>
        /// <response code="200">Compras obtenidas exitosamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("obtener-compras")]
        [ProducesResponseType(typeof(JsonApiResponse<List<CompraDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerCompras()
        {
            var compras = await _compraService.ObtenerCompras();
            return JsonApiResponseFactory.Success(compras, "compra", "Compras obtenidas correctamente");
        }

        /// <summary>
        /// Obtiene una compra específica por su identificador.
        /// </summary>
        /// <param name="id">Identificador único de la compra.</param>
        /// <returns>Datos de la compra solicitada.</returns>
        /// <response code="200">Compra obtenida exitosamente.</response>
        /// <response code="404">No se encontró la compra con el Id proporcionado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("obtener-compra-por-id/{id}")]
        [ProducesResponseType(typeof(JsonApiResponse<CompraDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(JsonApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(JsonApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerCompraPorId(int id)
        {
            var compra = await _compraService.ObtenerCompraPorId(id);
            return JsonApiResponseFactory.Success(compra, "compra", $"Compra {id} obtenida correctamente");
        }
    }
}
