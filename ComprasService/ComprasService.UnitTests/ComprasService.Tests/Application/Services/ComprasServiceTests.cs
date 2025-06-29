using AutoMapper;
using Moq;
using ComprasService.Application.Dto;
using ComprasService.Application.Interfaces;
using ComprasService.Application.Services;
using ComprasService.Domain.Entity;
using ComprasService.Domain.Interfaces.Repository;
using System.Linq.Expressions;
using Xunit;
using ComprasService.Application.Dto.Integrations;
using ComprasService.Application.Interfaces.Integrations;

namespace ComprasService.UnitTests.ComprasService.Tests.Application.Services
{
    public class CompraServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IProductoApiService> _productoApiServiceMock;
        private readonly Mock<IInventarioApiService> _inventarioApiServiceMock;
        private readonly CompraService _compraService;

        public CompraServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _productoApiServiceMock = new Mock<IProductoApiService>();
            _inventarioApiServiceMock = new Mock<IInventarioApiService>();

            _currentUserServiceMock.Setup(x => x.GetCurrentUserId()).Returns(1);
            _currentUserServiceMock.Setup(x => x.GetCurrentToken()).Returns("token");

            _compraService = new CompraService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object,
                _productoApiServiceMock.Object,
                _inventarioApiServiceMock.Object);
            
        }

        [Fact]
        public async Task RegistrarCompra_DeberiaRegistrarCorrectamente()
        {
            var compraDto = new RegistraCompraDto
            {
                IdCliente = 1,
                Productos = new List<RegistrarCompraDetalleDto>
                {
                    new RegistrarCompraDetalleDto { IdProducto = 10, CantidadProducto = 2 }
                }
            };

            _productoApiServiceMock.Setup(x => x.ObtenerProductoPorIdAsync(10, "token"))
                .ReturnsAsync(new ProductoDto { IdProducto = 10, NombreProducto = "Prod", Precio = 100, Activo = true });

            var crudMock = new Mock<IGenericRepository<Compra>>();
            _unitOfWorkMock.Setup(x => x.Crud<Compra>()).Returns(crudMock.Object);

            _mapperMock.Setup(x => x.Map<CompraDto>(It.IsAny<Compra>()))
                .Returns(new CompraDto { IdCliente = 1 });

            var result = await _compraService.RegistrarCompra(compraDto);

            crudMock.Verify(x => x.Add(It.IsAny<Compra>()));
            _unitOfWorkMock.Verify(x => x.SaveChanges());

            Assert.NotNull(result);
            Assert.Equal(1, result.IdCliente);
        }

        [Fact]
        public async Task RegistrarCompra_DeberiaLanzarSiProductoNoExiste()
        {
            var compraDto = new RegistraCompraDto
            {
                IdCliente = 1,
                Productos = new List<RegistrarCompraDetalleDto>
                {
                    new RegistrarCompraDetalleDto { IdProducto = 10, CantidadProducto = 2 }
                }
            };

            _productoApiServiceMock.Setup(x => x.ObtenerProductoPorIdAsync(10, "token"))
                .ReturnsAsync((ProductoDto)null);

            await Assert.ThrowsAsync<ApplicationException>(() => _compraService.RegistrarCompra(compraDto));
        }

        [Fact]
        public async Task RegistrarCompra_DeberiaLanzarSiProductoInactivo()
        {
            var compraDto = new RegistraCompraDto
            {
                IdCliente = 1,
                Productos = new List<RegistrarCompraDetalleDto>
                {
                    new RegistrarCompraDetalleDto { IdProducto = 10, CantidadProducto = 2 }
                }
            };

            _productoApiServiceMock.Setup(x => x.ObtenerProductoPorIdAsync(10, "token"))
                .ReturnsAsync(new ProductoDto { IdProducto = 10, Activo = false });

            await Assert.ThrowsAsync<ApplicationException>(() => _compraService.RegistrarCompra(compraDto));
        }

        [Fact]
        public async Task ObtenerCompras_DeberiaRetornarListaDeCompras()
        {
            var compras = new List<Compra>
            {
                new Compra
                {
                    IdCompra = 1,
                    CompraDetalles = new List<CompraDetalle>
                    {
                        new CompraDetalle { IdProducto = 10, CantidadProducto = 1 }
                    }
                }
            };

            var crudMock = new Mock<IGenericRepository<Compra>>();
            crudMock.Setup(x => x.GetAllWithIncludes(It.IsAny<Expression<Func<Compra, object>>[]>()))
                .Returns(compras.AsQueryable());

            _unitOfWorkMock.Setup(x => x.Crud<Compra>()).Returns(crudMock.Object);
            _mapperMock.Setup(x => x.Map<CompraDto>(It.IsAny<Compra>()))
                .Returns(new CompraDto { IdCliente = 1 });

            _productoApiServiceMock.Setup(x => x.ObtenerProductoPorIdAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new ProductoDto { IdProducto = 10, NombreProducto = "Prod", Precio = 100, Activo = true });

            var result = await _compraService.ObtenerCompras();

            Assert.Single(result);
            Assert.Equal(1, result[0].IdCliente);
        }

        [Fact]
        public async Task ObtenerCompras_DeberiaRetornarListaVacia()
        {
            var crudMock = new Mock<IGenericRepository<Compra>>();
            crudMock.Setup(x => x.GetAllWithIncludes(It.IsAny<Expression<Func<Compra, object>>[]>()))
                .Returns(new List<Compra>().AsQueryable());

            _unitOfWorkMock.Setup(x => x.Crud<Compra>()).Returns(crudMock.Object);

            var result = await _compraService.ObtenerCompras();

            Assert.Empty(result);
        }

        [Fact]
        public async Task ObtenerCompraPorId_DeberiaRetornarCompra()
        {
            var compras = new List<Compra>
            {
                new Compra
                {
                    IdCompra = 1,
                    CompraDetalles = new List<CompraDetalle>
                    {
                        new CompraDetalle { IdProducto = 10, CantidadProducto = 1 }
                    }
                }
            };

            var crudMock = new Mock<IGenericRepository<Compra>>();
            crudMock.Setup(x => x.FindWithIncludes(It.IsAny<Expression<Func<Compra, bool>>>(), It.IsAny<Expression<Func<Compra, object>>[]>()))
                .Returns(compras.AsQueryable());

            _unitOfWorkMock.Setup(x => x.Crud<Compra>()).Returns(crudMock.Object);

            _mapperMock.Setup(x => x.Map<CompraDto>(It.IsAny<Compra>()))
                .Returns(new CompraDto { IdCliente = 1 });

            _productoApiServiceMock.Setup(x => x.ObtenerProductoPorIdAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new ProductoDto { IdProducto = 10, NombreProducto = "Prod", Precio = 100, Activo = true });

            var result = await _compraService.ObtenerCompraPorId(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.IdCliente);
        }

        [Fact]
        public async Task ObtenerCompraPorId_DeberiaLanzarSiNoExiste()
        {
            var crudMock = new Mock<IGenericRepository<Compra>>();
            crudMock.Setup(x => x.FindWithIncludes(It.IsAny<Expression<Func<Compra, bool>>>(), It.IsAny<Expression<Func<Compra, object>>[]>()))
                .Returns(new List<Compra>().AsQueryable());

            _unitOfWorkMock.Setup(x => x.Crud<Compra>()).Returns(crudMock.Object);

            await Assert.ThrowsAsync<ApplicationException>(() => _compraService.ObtenerCompraPorId(99));
        }
    }
}
