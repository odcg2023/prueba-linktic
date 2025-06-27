using AutoMapper;
using SeguridadService.Application.Dto;
using SeguridadService.Application.Interfaces;
using SeguridadService.Domain.Interfaces.Repository;

namespace SeguridadService.Application.Services
{
    public class LoginService : ServiceBase, ILoginService
    {
        public LoginService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            
        }
        public LoginResponseDto Login(LoginRequestDto login)
        {
            throw new NotImplementedException();
        }
    }
}
