using AutoMapper;
using Moq;
using SeguridadService.Application.Dto;
using SeguridadService.Application.Interfaces;
using SeguridadService.Application.Services;
using SeguridadService.Common;
using SeguridadService.Domain.Entity;
using SeguridadService.Domain.Interfaces.Repository;
using System.Linq.Expressions;
using Xunit;
using static SeguridadService.Common.AppConstants;

namespace SeguridadService.UnitTests.SeguridadService.Tests.Application.Services
{
    public class SeguridadServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        public SeguridadServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task Login_CuandoLoginEsNull_LanzaApplicationException()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockCrypto = new Mock<ICryptoHelper>();

            var service = new LoginService(mockUow.Object, mockMapper.Object, mockCrypto.Object);

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                service.Login(null));

            Assert.Equal(AppConstants.Messages.CredencialesInvalidas, ex.Message);
        }

        [Fact]
        public async Task Login_CuandoUsuarioNoExiste_LanzaApplicationException()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockCrypto = new Mock<ICryptoHelper>();

            mockUow.Setup(u => u.Crud<Usuario>()
                .Find(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(new List<Usuario>().AsQueryable());

            var service = new LoginService(mockUow.Object, mockMapper.Object, mockCrypto.Object);

            var login = new LoginRequestDto
            {
                UsuarioLogin = "noexiste",
                Password = "123"
            };

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                service.Login(login));

            Assert.Equal(AppConstants.Messages.LogueoFallido, ex.Message);
        }

        [Fact]
        public async Task Login_CuandoUsuarioInactivo_LanzaApplicationException()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockCrypto = new Mock<ICryptoHelper>();

            var usuario = new Usuario
            {
                Login = "test",
                Activo = false,
                Password = "encrypted"
            };

            mockUow.Setup(u => u.Crud<Usuario>()
                .Find(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(new List<Usuario> { usuario }.AsQueryable());

            var service = new LoginService(mockUow.Object, mockMapper.Object, mockCrypto.Object);

            var login = new LoginRequestDto
            {
                UsuarioLogin = "test",
                Password = "123"
            };

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                service.Login(login));

            Assert.Equal(AppConstants.Messages.LogueoFallido, ex.Message);
        }

        [Fact]
        public async Task Login_CuandoPasswordIncorrecto_LanzaApplicationException()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockCrypto = new Mock<ICryptoHelper>();

            var usuario = new Usuario
            {
                Login = "test",
                Activo = true,
                Password = "encrypted"
            };

            mockUow.Setup(u => u.Crud<Usuario>()
                .Find(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(new List<Usuario> { usuario }.AsQueryable());

            mockCrypto.Setup(x => x.Decrypt("123")).Returns("123");
            mockCrypto.Setup(x => x.Decrypt("encrypted")).Returns("456");

            var service = new LoginService(mockUow.Object, mockMapper.Object, mockCrypto.Object);

            var login = new LoginRequestDto
            {
                UsuarioLogin = "test",
                Password = "123"
            };

            var ex = await Assert.ThrowsAsync<ApplicationException>(() =>
                service.Login(login));

            Assert.Equal(AppConstants.Messages.LogueoFallido, ex.Message);
        }

        [Fact]
        public async Task Login_CuandoTodoCorrecto_RetornaLoginResponseDto()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockCrypto = new Mock<ICryptoHelper>();

            var usuario = new Usuario
            {
                IdUsuario = 1,
                Login = "test",
                Nombre = "Juan",
                Activo = true,
                Password = "encrypted"
            };

            mockUow.Setup(u => u.Crud<Usuario>()
                .Find(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Returns(new List<Usuario> { usuario }.AsQueryable());

            mockCrypto.Setup(x => x.Decrypt("123")).Returns("123");
            mockCrypto.Setup(x => x.Decrypt("encrypted")).Returns("123");

            var service = new LoginService(mockUow.Object, mockMapper.Object, mockCrypto.Object);

            var login = new LoginRequestDto
            {
                UsuarioLogin = "test",
                Password = "123"
            };

            var result = await service.Login(login);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Juan", result.NombreUsuario);
        }

        [Fact]
        public async Task Login_CuandoHayErrorDeBaseDatos_LanzaExcepcion()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockCrypto = new Mock<ICryptoHelper>();

            // Simulamos que al intentar acceder a la base de datos se produce un error
            mockUow.Setup(u => u.Crud<Usuario>()
                .Find(It.IsAny<Expression<Func<Usuario, bool>>>()))
                .Throws(new InvalidOperationException("Error al conectar a la base de datos"));

            var service = new LoginService(mockUow.Object, mockMapper.Object, mockCrypto.Object);

            var login = new LoginRequestDto
            {
                UsuarioLogin = "test",
                Password = "123"
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.Login(login));

            Assert.Equal("Error al conectar a la base de datos", ex.Message);
        }
    }
}
