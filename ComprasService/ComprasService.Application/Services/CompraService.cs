using AutoMapper;
using ComprasService.Application.Dto;
using ComprasService.Application.Dto.Integrations;
using ComprasService.Application.Interfaces;
using ComprasService.Application.Interfaces.Integrations;
using ComprasService.Domain.Entity;
using ComprasService.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Services
{
    public class CompraService : ServiceBase, ICompraService
    {
        private readonly IProductoApiService _productoApiService;

        public CompraService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService,
            IProductoApiService productoApiService)
            : base(unitOfWork, mapper, currentUserService)
        {
            _productoApiService = productoApiService;
        }

        public async Task<CompraDto> RegistrarCompra(RegistraCompraDto nuevaCompra)
        {
            var nuevaCompraDb = CrearCompraEntity(nuevaCompra);
            var productosDto = await ProcesarDetallesCompra(nuevaCompra, nuevaCompraDb);

            UnidadTrabajo.Crud<Compra>().Add(nuevaCompraDb);
            UnidadTrabajo.SaveChanges();

            var compraDto = Mapper.Map<CompraDto>(nuevaCompraDb);
            compraDto.Productos = productosDto;

            return compraDto;
        }

        public async Task<List<CompraDto>> ObtenerCompras()
        {
            var comprasDb =  UnidadTrabajo.Crud<Compra>().GetAllWithIncludes(x=> x.CompraDetalles);
            var comprasDto = new List<CompraDto>();

            foreach (var compra in comprasDb)
            {
                var compraDto = Mapper.Map<CompraDto>(compra);
                compraDto.Productos = await EnriquecerDetallesCompra(compra.CompraDetalles);
                comprasDto.Add(compraDto);
            }

            return comprasDto;
        }

        public async Task<CompraDto> ObtenerCompraPorId(int idCompra)
        {
            var compraDb =  UnidadTrabajo.Crud<Compra>().FindWithIncludes(x => x.IdCompra == idCompra, x=> x.CompraDetalles).FirstOrDefault()
                            ?? throw new ApplicationException($"No existe la compra con Id: {idCompra}");

            var compraDto = Mapper.Map<CompraDto>(compraDb);
            compraDto.Productos = await EnriquecerDetallesCompra(compraDb.CompraDetalles);

            return compraDto;
        }

        private Compra CrearCompraEntity(RegistraCompraDto compra)
        {
            return new Compra
            {
                IdCliente = compra.IdCliente,
                TotalItems = compra.Productos.Count,
                UsuarioCreacion = CurrentUserService.GetCurrentUserId(),
                FechaCreacion = DateTime.UtcNow
            };
        }

        private async Task<List<CompraDetalleDto>> ProcesarDetallesCompra(
            RegistraCompraDto compraDto, Compra compraEntity)
        {
            var productos = new List<CompraDetalleDto>();

            foreach (var item in compraDto.Productos)
            {
                var producto = await ValidarProducto(item.IdProducto);

                var detalle = new CompraDetalle
                {
                    IdProducto = item.IdProducto,
                    CantidadProducto = item.CantidadProducto,
                    UsuarioCreacion = CurrentUserService.GetCurrentUserId(),
                    FechaCreacion = DateTime.UtcNow
                };

                compraEntity.CompraDetalles.Add(detalle);
                compraEntity.ValorTotalCompra += producto.Precio * item.CantidadProducto;

                productos.Add(new CompraDetalleDto
                {
                    IdProducto = item.IdProducto,
                    NombreProducto = producto.NombreProducto,
                    CantidadProducto = item.CantidadProducto,
                    PrecioUnitario = producto.Precio
                });
            }

            return productos;
        }

        private async Task<List<CompraDetalleDto>> EnriquecerDetallesCompra(
            ICollection<CompraDetalle> detalles)
        {
            var productos = new List<CompraDetalleDto>();

            foreach (var detalle in detalles)
            {
                var producto = await ValidarProducto(detalle.IdProducto);

                productos.Add(new CompraDetalleDto
                {
                    IdProducto = detalle.IdProducto,
                    NombreProducto = producto.NombreProducto,
                    CantidadProducto = detalle.CantidadProducto,
                    PrecioUnitario = producto.Precio
                });
            }

            return productos;
        }

        private async Task<ProductoDto> ValidarProducto(int idProducto)
        {
            var producto = await _productoApiService.ObtenerProductoPorIdAsync(
                idProducto, CurrentUserService.GetCurrentToken());

            if (producto == null)
                throw new ApplicationException($"No existe el producto con Id: {idProducto}");

            if (!producto.Activo)
                throw new ApplicationException($"El producto con Id: {idProducto} está inactivo");

            return producto;
        }
    }

}
