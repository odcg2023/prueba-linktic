using AutoMapper;
using Moq;
using ProductosService.Application.Dto;
using ProductosService.Application.Interfaces;
using ProductosService.Application.Services;
using ProductosService.Domain.Entity;
using ProductosService.Domain.Interfaces.Repository;
using System.Linq.Expressions;
using Xunit;

namespace ProductosService.UnitTests.ProductosService.Tests.Application.Services
{
    public class ProductoServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ProductoService _productoService;

        public ProductoServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            // Siempre devuelve un id de usuario simulado
            _currentUserServiceMock.Setup(c => c.GetCurrentUserId()).Returns(1);

            _productoService = new ProductoService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task ObtenerProductoPorId_RetornaProductoDto()
        {
            // Arrange
            var producto = new Producto { IdProducto = 1, NombreProducto = "Producto1" };

            _unitOfWorkMock.Setup(u => u.Crud<Producto>().Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                .Returns(new List<Producto> { producto }.AsQueryable());

            _mapperMock.Setup(m => m.Map<ProductoDto>(It.IsAny<Producto>()))
                .Returns(new ProductoDto { IdProducto = 1, NombreProducto = "Producto1" });

            // Act
            var resultado = await _productoService.ObtenerProductoPorId(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.IdProducto);
            Assert.Equal("Producto1", resultado.NombreProducto);
        }

        [Fact]
        public async Task ObtenerProductoPorId_ProductoNoExiste_DeberiaRetornarNulo()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Crud<Producto>().Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                .Returns(Enumerable.Empty<Producto>().AsQueryable());

            // Act
            var resultado = await _productoService.ObtenerProductoPorId(999);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task CrearProducto_ProductoValido_DeberiaCrearYRetornarProductoDto()
        {
            // Arrange
            var productoNuevoDto = new ProductoNuevoDto
            {
                NombreProducto = "Nuevo Producto",
                Descripcion = "Prueba",
                Precio = 10000
            };

            var productoEntidad = new Producto
            {
                IdProducto = 1,
                NombreProducto = "Nuevo Producto",
                Descripcion = "Prueba",
                Precio = 10000,
                Activo = true
            };

            _unitOfWorkMock.Setup(u => u.Crud<Producto>().Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                .Returns(Enumerable.Empty<Producto>().AsQueryable());

            _unitOfWorkMock.Setup(u => u.Crud<Producto>().Add(It.IsAny<Producto>()))
                .Returns(productoEntidad);

            _unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(1);

            _mapperMock.Setup(m => m.Map<Producto>(productoNuevoDto)).Returns(productoEntidad);
            _mapperMock.Setup(m => m.Map<ProductoDto>(productoEntidad)).Returns(new ProductoDto
            {
                IdProducto = 1,
                NombreProducto = "Nuevo Producto",
                Precio = 10000
            });

            // Act
            var resultado = await _productoService.CrearProducto(productoNuevoDto);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.IdProducto);
            Assert.Equal("Nuevo Producto", resultado.NombreProducto);
        }

        [Fact]
        public async Task CrearProducto_NombreDuplicado_DeberiaLanzarExcepcion()
        {
            // Arrange
            var productoDto = new ProductoNuevoDto { NombreProducto = "ProductoExistente" };
            var productoExistente = new Producto { NombreProducto = "ProductoExistente" };

            _unitOfWorkMock.Setup(u => u.Crud<Producto>().Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                .Returns(new List<Producto> { productoExistente }.AsQueryable());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                _productoService.CrearProducto(productoDto));

            Assert.Contains("Ya existe un producto con el nombre", ex.Message);
        }

        [Fact]
        public async Task ObtenerProductos_SinProductos_DeberiaRetornarListaVacia()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.Crud<Producto>().GetAll())
                .Returns(Enumerable.Empty<Producto>().AsQueryable());

            _mapperMock.Setup(m => m.Map<List<ProductoDto>>(It.IsAny<List<Producto>>()))
                .Returns(new List<ProductoDto>());

            // Act
            var resultado = await _productoService.ObtenerProductos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task ObtenerProductos_ConProductos_DeberiaRetornarLista()
        {
            // Arrange
            var productos = new List<Producto>
        {
            new Producto { IdProducto = 1, NombreProducto = "Producto A" },
            new Producto { IdProducto = 2, NombreProducto = "Producto B" }
        };

            var productosDto = new List<ProductoDto>
        {
            new ProductoDto { IdProducto = 1, NombreProducto = "Producto A" },
            new ProductoDto { IdProducto = 2, NombreProducto = "Producto B" }
        };

            _unitOfWorkMock.Setup(u => u.Crud<Producto>().GetAll())
                .Returns(productos.AsQueryable());

            _mapperMock.Setup(m => m.Map<List<ProductoDto>>(productos)).Returns(productosDto);

            // Act
            var resultado = await _productoService.ObtenerProductos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

        [Fact]
        public async Task CrearProducto_ErrorEnGuardar_DeberiaLanzarExcepcion()
        {
            // Arrange
            var productoDto = new ProductoNuevoDto { NombreProducto = "Producto Error" };
            var productoEntidad = new Producto { NombreProducto = "Producto Error" };

            _unitOfWorkMock.Setup(u => u.Crud<Producto>().Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                .Returns(Enumerable.Empty<Producto>().AsQueryable());

            _unitOfWorkMock.Setup(u => u.Crud<Producto>().Add(It.IsAny<Producto>()))
                .Returns(productoEntidad);

            _unitOfWorkMock.Setup(u => u.SaveChanges())
                .Throws(new Exception("Fallo al guardar"));

            _mapperMock.Setup(m => m.Map<Producto>(productoDto)).Returns(productoEntidad);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _productoService.CrearProducto(productoDto));
        }

        [Fact]
        public async Task ObtenerProductoPorId_ErrorBaseDatos_DeberiaLanzarExcepcion()
        {
            // Arrange
            var idProducto = 1;

            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                    .Throws(new Exception("Error en base de datos"));

            _unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _productoService.ObtenerProductoPorId(idProducto));
        }

        [Fact]
        public async Task ObtenerProductos_ErrorBaseDatos_DeberiaLanzarExcepcion()
        {
            // Arrange
            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.GetAll()).Throws(new Exception("Error en repositorio"));

            _unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _productoService.ObtenerProductos());
        }


    }

}
