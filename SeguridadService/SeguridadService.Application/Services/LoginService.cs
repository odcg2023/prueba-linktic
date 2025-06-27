using AutoMapper;
using SeguridadService.Application.Dto;
using SeguridadService.Application.Interfaces;
using SeguridadService.Common;
using SeguridadService.Domain.Entity;
using SeguridadService.Domain.Interfaces.Repository;
using static SeguridadService.Common.AppConstants;

namespace SeguridadService.Application.Services
{
    public class LoginService : ServiceBase, ILoginService
    {
        public LoginService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            
        }
        public Task<LoginResponseDto> Login(LoginRequestDto login)
        {
            Validaciones(login);

            var usuario = UnidadTrabajo
                .Crud<Usuario>()
                .Find(x => x.Login.ToLower() == login.UsuarioLogin.Trim().ToLower())
                .FirstOrDefault();

            if (usuario == null)
                throw new ApplicationException(Messages.LogueoFallido);

            ValidarUsuarioActivo(usuario);
            ValidarPassword(login, usuario);

            return Task.FromResult(new LoginResponseDto
            {
                Id = usuario.IdUsuario,
                NombreUsuario = usuario.Nombre
            });
        }

        private static void Validaciones(LoginRequestDto login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.UsuarioLogin) || string.IsNullOrWhiteSpace(login.Password))
                throw new ApplicationException(Messages.CredencialesInvalidas);
        }

        private void ValidarUsuarioActivo(Usuario usuario)
        {
            if(!usuario.Activo.GetValueOrDefault())
                throw new ApplicationException(Messages.LogueoFallido);
        }

        private void ValidarPassword(LoginRequestDto login, Usuario usuario)
        {
            var password = Crypto.Decrypt(login.Password);
            var passwordDb = Crypto.Decrypt(usuario.Password);

            if (passwordDb != password)
                throw new ApplicationException(Messages.LogueoFallido);
        }

    }
}
