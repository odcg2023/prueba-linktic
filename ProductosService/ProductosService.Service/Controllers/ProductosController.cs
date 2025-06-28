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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;
        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet("obtener-producto-por-id")]
        public async Task<IActionResult> ObtenerProductoPorId(int idProducto)
        {
            var producto = await _productoService.ObtenerProductoPorId(idProducto);

            if (producto == null)
                return JsonApiResponseFactory.NotFound(Messages.ProductoInexistente);

            return JsonApiResponseFactory.Success(producto, "producto", Messages.PeticionCorrecta);
        }

        [HttpGet("obtener-productos")]
        public async Task<IActionResult> ObtenerProductos()
        {
            var listaProductos = await _productoService.ObtenerProductos();
            return JsonApiResponseFactory.Success(listaProductos, "producto", Messages.PeticionCorrecta);
        }

        [HttpPost("crear-producto")]
        public async Task<IActionResult> CrearProducto(ProductoNuevoDto producto)
        {
            var resultadoCreacion = await _productoService.CrearProducto(producto);
            return JsonApiResponseFactory.Success(resultadoCreacion, "producto", Messages.PeticionCorrecta);
        }
    }
}
