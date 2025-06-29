using AutoMapper;
using InventariosService.Application.Dto;
using InventariosService.Application.Dto.Integrations;
using InventariosService.Application.Enumerations;
using InventariosService.Application.Interfaces;
using InventariosService.Application.Interfaces.Integrations;
using InventariosService.Domain.Entity;
using InventariosService.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosService.Application.Services
{
    public class InventarioService : ServiceBase, IInventarioService
    {
        private readonly IProductoApiService _productoApiService;
        public InventarioService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService,
            IProductoApiService productoApiService)
            : base(unitOfWork, mapper, currentUserService)
        {
            _productoApiService = productoApiService;
        }

        public async Task<InventarioProductoDto> ActualizarInventarioProducto(ActualizaInventarioDto inventario)
        {
            await ValidarProducto(inventario.Producto.IdProducto);
            ValidarTipoMovimientoExistente(inventario.TipoMovimiento);
            var inventarioDb = ActualizarInventario(inventario);
            return Mapper.Map<InventarioProductoDto>(inventarioDb);
        }

        public async Task<InventarioCompraActualizadoDto> ActualizarInventarioPorCompras(ProductosCompraDto productos)
        {
            using var transaction = UnidadTrabajo.BeginTransaction();

            try
            {
                ValidarTipoMovimientoExistente(productos.TipoMovimiento);
                ValidarTipoMovimientoCompra(productos.TipoMovimiento);

                var inventarioMovimientoDb = GuardarInventarioMovimiento(new InventarioMovimientoDto
                {
                    TipoMovimiento = productos.TipoMovimiento,
                    Observaciones = productos.Observaciones
                });

                await ProcesarListaProductosCompra(productos, inventarioMovimientoDb);

                transaction.Commit();

                return new InventarioCompraActualizadoDto
                {
                    IdMovimiento = inventarioMovimientoDb.IdMovimiento,
                    TipoMovimiento = (TipoMovimiento)inventarioMovimientoDb.TipoMovimiento
                };
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public Task<InventarioProductoDto> ObtenerInventarioProducto(int idProducto)
        {
            var inventarioActual = UnidadTrabajo.Crud<Inventario>()
                .Find(x => x.IdProducto == idProducto).FirstOrDefault()
                ?? throw new ApplicationException($"No se encuentra el inventario para el producto con Id: {idProducto}");

            return Task.FromResult(Mapper.Map<InventarioProductoDto>(inventarioActual));
        }

        private async Task ProcesarListaProductosCompra(ProductosCompraDto productos, InventarioMovimiento inventarioMovimientoDb)
        {
            foreach (var producto in productos.ListaProductos)
            {
                await ValidarProducto(producto.IdProducto);
                var inventarioDb = GuardarInventario(
                    new ActualizaInventarioDto
                    {
                        Producto = producto,
                        TipoMovimiento = productos.TipoMovimiento
                    });

                GuardarInventarioMovimientoDetalle(
                    inventarioDb,
                    new ActualizaInventarioDto
                    {
                        Producto = producto,
                        TipoMovimiento = productos.TipoMovimiento
                    },
                    inventarioMovimientoDb);
            }
        }

        private void ValidarTipoMovimientoCompra(TipoMovimiento tipoMovimiento)
        {
            if (tipoMovimiento != TipoMovimiento.SALIDA)
            {
                throw new ApplicationException($"No se puede actualizar el inventario " +
                    $"si el movimiento no es de salida. Es decir una compra.");
            }
        }

        private Inventario ActualizarInventario(ActualizaInventarioDto inventario)
        {
            using var transaction = UnidadTrabajo.BeginTransaction();

            try
            {
                Inventario? inventarioDb = GuardarInventario(inventario);

                var inventarioMoovimientoDb = GuardarInventarioMovimiento(new InventarioMovimientoDto
                {
                    TipoMovimiento = inventario.TipoMovimiento,
                });

                GuardarInventarioMovimientoDetalle(inventarioDb, inventario, inventarioMoovimientoDb);
                transaction.Commit();
                return inventarioDb;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private Inventario GuardarInventario(ActualizaInventarioDto inventario)
        {
            var inventarioDb = UnidadTrabajo.Crud<Inventario>()
                                        .Find(x => x.IdProducto == inventario.Producto.IdProducto)
                                        .FirstOrDefault();

            if (inventarioDb == null)
            {
                inventarioDb = CrearNuevoInventario(inventario);
                UnidadTrabajo.Crud<Inventario>().Add(inventarioDb);
            }
            else
            {
                ActualizarInventarioExistente(inventarioDb, inventario);
                UnidadTrabajo.Crud<Inventario>().Edit(inventarioDb);
            }

            UnidadTrabajo.SaveChanges();
            return inventarioDb;
        }

        private InventarioMovimiento GuardarInventarioMovimiento(InventarioMovimientoDto movimiento)
        {
            var inventarioMovimientoDb = new InventarioMovimiento
            {
                TipoMovimiento = (byte)movimiento.TipoMovimiento,
                UsuarioCreacion = CurrentUserService.GetCurrentUserId(),
                Observaciones = movimiento.Observaciones
            };

            UnidadTrabajo.Crud<InventarioMovimiento>().Add(inventarioMovimientoDb);
            UnidadTrabajo.SaveChanges();
            return inventarioMovimientoDb;
        }

        private static void ValidarTipoMovimientoExistente(TipoMovimiento tipoMovimiento)
        {
            if (!Enum.IsDefined(typeof(TipoMovimiento), tipoMovimiento))
            {
                throw new ApplicationException($"No existe el tipo de movimiento recibido. " +
                    $"Solo existen: {TipoMovimiento.ENTRADA} y {TipoMovimiento.SALIDA}.");
            }
        }

        private async Task<ProductoDto> ValidarProducto(int idProducto)
        {
            var producto = await _productoApiService
                 .ObtenerProductoPorIdAsync(idProducto, CurrentUserService.GetCurrentToken())
                 ?? throw new ApplicationException($"No existe el producto con Id: {idProducto}");

            if (!producto.Activo)
                throw new ApplicationException($"El producto con Id: {idProducto} está inactivo");

            return producto;
        }

        private Inventario CrearNuevoInventario(ActualizaInventarioDto inventario)
        {
            if (inventario.TipoMovimiento != TipoMovimiento.ENTRADA)
            {
                throw new ApplicationException($"No se puede iniciar un inventario con una salida. " +
                    $"Debe realizar primero una entrada.");
            }

            return new Inventario
            {
                IdProducto = inventario.Producto.IdProducto,
                ExistenciasActuales = inventario.Producto.Cantidad,
                UsuarioCreacion = CurrentUserService.GetCurrentUserId()
            };
        }

        private void ActualizarInventarioExistente(Inventario inventarioDb, ActualizaInventarioDto inventario)
        {
            var existenciasAntes = inventarioDb.ExistenciasActuales;

            if (inventario.TipoMovimiento == TipoMovimiento.ENTRADA)
            {
                inventarioDb.ExistenciasActuales += inventario.Producto.Cantidad;
            }
            else if (inventario.TipoMovimiento == TipoMovimiento.SALIDA)
            {
                if (inventario.Producto.Cantidad > existenciasAntes)
                {
                    throw new ApplicationException($"No hay existencias suficientes. Actualmente hay {existenciasAntes}.");
                }
                inventarioDb.ExistenciasActuales -= inventario.Producto.Cantidad;
            }

            inventarioDb.UsuarioModificacion = CurrentUserService.GetCurrentUserId();
            inventarioDb.FechaModificacion = DateTime.UtcNow;
        }

        private void GuardarInventarioMovimientoDetalle(
            Inventario inventarioDb,
            ActualizaInventarioDto inventario,
            InventarioMovimiento movimiento)
        {
            var movimientoDetalle = new InventarioMovimientoDetalle
            {
                IdMovimiento = movimiento.IdMovimiento,
                IdInventario = inventarioDb.IdInventario,
                CantidadAntes = CalcularCantidadAntes(inventarioDb.ExistenciasActuales, inventario),
                CantidadMovimiento = inventario.Producto.Cantidad,
                CantidadDespues = inventarioDb.ExistenciasActuales,
                UsuarioCreacion = CurrentUserService.GetCurrentUserId(),
            };

            UnidadTrabajo.Crud<InventarioMovimientoDetalle>().Add(movimientoDetalle);
            UnidadTrabajo.SaveChanges();
        }
        private int CalcularCantidadAntes(int existenciasActuales, ActualizaInventarioDto inventario)
        {
            return inventario.TipoMovimiento == TipoMovimiento.ENTRADA
                ? existenciasActuales - inventario.Producto.Cantidad
                : existenciasActuales + inventario.Producto.Cantidad;
        }

    }
}
