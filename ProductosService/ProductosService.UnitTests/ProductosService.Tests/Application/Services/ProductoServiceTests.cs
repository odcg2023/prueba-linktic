using AutoMapper;
using Moq;
using ProductosService.Application.Dto;
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
        private readonly ProductoService _productoService;

        public ProductoServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _productoService = new ProductoService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task ObtenerProductoPorId_RetornaProductoDto()
        {
            // Arrange
            var producto = new Producto { IdProducto = 1, NombreProducto = "Producto1" };
            _unitOfWorkMock.Setup(u => u.Crud<Producto>().Find(It.IsAny<System.Linq.Expressions.Expression<Func<Producto, bool>>>()))
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
            var idProducto = 999;

            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                    .Returns(Enumerable.Empty<Producto>().AsQueryable()); // Simula que no existe

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);

            var mapperMock = new Mock<IMapper>(); // No se necesita configurar mapeo

            var service = new ProductoService(unitOfWorkMock.Object, mapperMock.Object);

            // Act
            var resultado = await service.ObtenerProductoPorId(idProducto);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task CrearProducto_ProductoValido_DeberiaCrearYRetornarId()
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

            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                    .Returns(Enumerable.Empty<Producto>().AsQueryable());

            repoMock.Setup(r => r.Add(It.IsAny<Producto>()))
                    .Returns(productoEntidad);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);
            unitOfWorkMock.Setup(u => u.SaveChanges()).Returns(1);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Producto>(productoNuevoDto)).Returns(productoEntidad);

            var service = new ProductoService(unitOfWorkMock.Object, mapperMock.Object);

            // Act
            var resultado = await service.CrearProducto(productoNuevoDto);

            // Assert
            Assert.Equal(1, resultado);
        }

        [Fact]
        public async Task CrearProducto_NombreDuplicado_DeberiaLanzarExcepcion()
        {
            // Arrange
            var productoDto = new ProductoNuevoDto
            {
                NombreProducto = "ProductoExistente"
            };

            var productoExistente = new Producto { NombreProducto = "ProductoExistente" };

            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                    .Returns(new List<Producto> { productoExistente }.AsQueryable());

            _unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                _productoService.CrearProducto(productoDto));

            Assert.Contains("Ya existe un producto con el nombre", ex.Message);
        }

        [Fact]
        public Task ObtenerProductos_SinProductos_DeberiaRetornarListaVacia()
        {
            // Arrange
            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.GetAll()).Returns(Enumerable.Empty<Producto>().AsQueryable());

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<List<ProductoDto>>(It.IsAny<List<Producto>>()))
                      .Returns(new List<ProductoDto>());

            var service = new ProductoService(unitOfWorkMock.Object, mapperMock.Object);

            // Act
            var resultado = service.ObtenerProductos();

            // Assert
            Assert.NotNull(resultado);
            return Task.CompletedTask;
        }

        [Fact]
        public Task ObtenerProductos_ConProductos_DeberiaRetornarLista()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new Producto { IdProducto = 1, NombreProducto = "Producto A", Precio = 1000 },
                new Producto { IdProducto = 2, NombreProducto = "Producto B", Precio = 2000 }
            };

            var productosDto = new List<ProductoDto>
            {
                new ProductoDto { IdProducto = 1, NombreProducto = "Producto A", Precio = 1000 },
                new ProductoDto { IdProducto = 2, NombreProducto = "Producto B", Precio = 2000 }
            };

            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.GetAll()).Returns(productos.AsQueryable());

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<List<ProductoDto>>(productos)).Returns(productosDto);

            var service = new ProductoService(unitOfWorkMock.Object, mapperMock.Object);

            // Act
            var resultado = service.ObtenerProductos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Result.Count);
            return Task.CompletedTask;
        }

        [Fact]
        public async Task ObtenerProductoPorId_ErrorBaseDatos_DeberiaLanzarExcepcion()
        {
            // Arrange
            var idProducto = 1;

            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                    .Throws(new Exception("Error en base de datos"));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);

            var mapperMock = new Mock<IMapper>();

            var service = new ProductoService(unitOfWorkMock.Object, mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.ObtenerProductoPorId(idProducto));
        }

        [Fact]
        public async Task CrearProducto_ErrorBaseDatos_DeberiaLanzarExcepcion()
        {
            // Arrange
            var productoNuevoDto = new ProductoNuevoDto
            {
                NombreProducto = "Error Producto",
                Precio = 9999
            };

            var productoEntidad = new Producto
            {
                NombreProducto = "Error Producto",
                Precio = 9999
            };

            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Producto, bool>>>()))
                    .Returns(Enumerable.Empty<Producto>().AsQueryable());

            repoMock.Setup(r => r.Add(It.IsAny<Producto>())).Returns(productoEntidad);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);
            unitOfWorkMock.Setup(u => u.SaveChanges()).Throws(new Exception("Fallo al guardar"));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Producto>(productoNuevoDto)).Returns(productoEntidad);

            var service = new ProductoService(unitOfWorkMock.Object, mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CrearProducto(productoNuevoDto));
        }

        [Fact]
        public async Task ObtenerProductos_ErrorBaseDatos_DeberiaLanzarExcepcion()
        {
            // Arrange
            var repoMock = new Mock<IGenericRepository<Producto>>();
            repoMock.Setup(r => r.GetAll()).Throws(new Exception("Error en repositorio"));

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Crud<Producto>()).Returns(repoMock.Object);

            var mapperMock = new Mock<IMapper>();

            var service = new ProductoService(unitOfWorkMock.Object, mapperMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.ObtenerProductos());
        }


    }
}
