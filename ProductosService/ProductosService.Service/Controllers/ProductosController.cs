using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductosService.Application.Dto;
using ProductosService.Application.Dto.JsonResponse;
using ProductosService.Application.Interfaces;
using ProductosService.Domain.Entity;
using ProductosService.Service.Helpers;
using static ProductosService.Common.AppConstants;

namespace ProductosService.Service.Controllers
{
    /// <summary>
    /// API para gestionar productos.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        /// <summary>
        /// Constructor del controlador de productos.
        /// </summary>
        /// <param name="productoService">Servicio de productos.</param>
        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        /// <summary>
        /// Obtiene un producto por su identificador único.
        /// </summary>
        /// <param name="idProducto">ID del producto a buscar.</param>
        /// <returns>Retorna un producto si existe, o un error 404 si no se encuentra.</returns>
        /// <response code="200">Producto encontrado exitosamente.</response>
        /// <response code="404">No existe un producto con el ID proporcionado.</response>
        [HttpGet("obtener-producto-por-id")]
        public async Task<IActionResult> ObtenerProductoPorId(int idProducto)
        {
            var producto = await _productoService.ObtenerProductoPorId(idProducto);

            if (producto == null)
                return JsonApiResponseFactory.NotFound(Messages.ProductoInexistente);

            return JsonApiResponseFactory.Success(producto, "producto", Messages.PeticionCorrecta);
        }

        /// <summary>
        /// Obtiene la lista de todos los productos registrados.
        /// </summary>
        /// <returns>Retorna una lista de productos.</returns>
        /// <response code="200">Lista obtenida exitosamente.</response>
        [HttpGet("obtener-productos")]
        public async Task<IActionResult> ObtenerProductos()
        {
            var listaProductos = await _productoService.ObtenerProductos();
            return JsonApiResponseFactory.Success(listaProductos, "producto", Messages.PeticionCorrecta);
        }

        /// <summary>
        /// Crea un nuevo producto en el sistema.
        /// </summary>
        /// <param name="producto">Datos del producto a crear.</param>
        /// <returns>Retorna el ID del producto creado o un mensaje de éxito.</returns>
        /// <response code="200">Producto creado exitosamente.</response>
        /// <response code="400">Error en los datos proporcionados.</response>
        [HttpPost("crear-producto")]
        public async Task<IActionResult> CrearProducto(ProductoNuevoDto producto)
        {
            var resultadoCreacion = await _productoService.CrearProducto(producto);
            return JsonApiResponseFactory.Success(resultadoCreacion, "producto", Messages.PeticionCorrecta);
        }
    }
}
