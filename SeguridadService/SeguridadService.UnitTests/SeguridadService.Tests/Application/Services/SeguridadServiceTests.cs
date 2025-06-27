using AutoMapper;
using Moq;
using SeguridadService.Application.Services;
using SeguridadService.Domain.Interfaces.Repository;
using System.Linq.Expressions;
using Xunit;

namespace SeguridadService.UnitTests.SeguridadService.Tests.Application.Services
{
    public class SeguridadServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly LoginService _loginService;

        public SeguridadServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loginService = new LoginService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        

    }
}
