using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductosService.Application.Dto;
using ProductosService.Application.Dto.JsonResponse;
using ProductosService.Application.Interfaces;
using ProductosService.Domain.Entity;

namespace ProductosService.Service.Controllers
{
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
            {
                return NotFound(new JsonApiErrorResponse
                {
                    Errors = new List<JsonApiError>
                    {
                        new JsonApiError
                        {
                            Status = "404",
                            Title = "Producto no encontrado",
                            Detail = $"No existe un producto con ID = {idProducto}"
                        }
                    },
                    Meta = new Meta
                    {
                        Success = false,
                        Message = "Error al obtener producto"
                    }
                });
            }

            return Ok(new JsonApiResponse<ProductoDto>
            {
                Data = new JsonApiData<ProductoDto>
                {
                    Type = "Productos",
                    Attributes = producto
                },
                Meta = new Meta
                {
                    Success = true,
                    Message = "Petición ejecutada de forma correcta"
                }
            });
        }

        [HttpGet("obtener-productos")]
        public async Task<IActionResult> ObtenerProductos()
        {
            var listaProductos = await _productoService.ObtenerProductos();

            return Ok(new JsonApiResponse<List<ProductoDto>>
            {
                Data = new JsonApiData<List<ProductoDto>>
                {
                    Type = "Productos",
                    Attributes = listaProductos
                },
                Meta = new Meta
                {
                    Success = true,
                    Message = "Petición ejecutada de forma correcta"
                }
            });
        }

        [HttpPost("crear-producto")]
        public async Task<IActionResult> CrearProducto(ProductoNuevoDto producto)
        {
            var resultadoCreacion = await _productoService.CrearProducto(producto);

            return Ok(new JsonApiResponse<int>
            {
                Data = new JsonApiData<int>
                {
                    Type = "Productos",
                    Attributes = resultadoCreacion
                },
                Meta = new Meta
                {
                    Success = true,
                    Message = "Producto creado correctamente"
                }
            });
        }
    }
}
