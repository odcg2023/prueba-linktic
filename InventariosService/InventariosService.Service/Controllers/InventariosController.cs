using InventariosService.Application.Dto;
using InventariosService.Application.Interfaces;
using InventariosService.Service.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventariosService.Service.Controllers
{

    [Authorize]
    /// <summary>
    /// API para gestionar el inventario de productos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _inventarioService;

        public InventarioController(IInventarioService inventarioService)
        {
            _inventarioService = inventarioService;
        }

        /// <summary>
        /// Actualiza las existencias de un producto en el inventario.
        /// </summary>
        /// <param name="inventario">Datos del inventario a actualizar.</param>
        /// <returns>El inventario actualizado del producto.</returns>
        /// <response code="200">Existencias del producto actualizadas exitosamente.</response>
        [HttpPost("actualizar-inventario")]
        [ProducesResponseType(typeof(InventarioProductoDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ActualizarInventarioProducto([FromBody] ActualizaInventarioDto inventario)
        {
            var result = await _inventarioService.ActualizarInventarioProducto(inventario);
            return JsonApiResponseFactory.Success(result, "inventario", "Existencias actualizadas correctamente");
        }

        [HttpPost("actualizar-inventario-compras")]
        [ProducesResponseType(typeof(InventarioProductoDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ActualizarInventarioCompras([FromBody] ProductosCompraDto compra)
        {
            var result = await _inventarioService.ActualizarInventarioPorCompras(compra);
            return JsonApiResponseFactory.Success(result, "inventario", "Existencias por compras actualizadas correctamente");
        }

        /// <summary>
        /// Obtiene las existencias actuales de un producto en el inventario.
        /// </summary>
        /// <param name="idProducto">Id del producto.</param>
        /// <returns>Existencias del producto en el inventario.</returns>
        /// <response code="200">Existencias del producto obtenidas exitosamente.</response>
        [HttpGet("obtener-existencias/{idProducto}")]
        [ProducesResponseType(typeof(InventarioProductoDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtenerExistenciasProductoInventario(int idProducto)
        {
            var result = await _inventarioService.ObtenerInventarioProducto(idProducto);
            return JsonApiResponseFactory.Success(result, "inventario", $"Existencias del producto {idProducto} obtenidas correctamente");
        }
    }

}
