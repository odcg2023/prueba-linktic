using InventariosService.Application.Interfaces;

namespace InventariosService.Service.Helpers
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICryptoHelper _crypto;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ICryptoHelper crypto)
        {
            _httpContextAccessor = httpContextAccessor;
            _crypto = crypto;
        }

        public short GetCurrentUserId()
        {
            var encrypted = _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == "Expresion1")?.Value;

            if (string.IsNullOrEmpty(encrypted))
                throw new ApplicationException("No se pudo obtener el usuario del token.");

            var decrypted = _crypto.Decrypt(encrypted);

            return short.Parse(decrypted);
        }
        public string GetCurrentToken()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader))
                throw new ApplicationException("No se pudo obtener el token del header Authorization.");

            if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return authorizationHeader.Substring("Bearer ".Length).Trim();

            return authorizationHeader;
        }
    }
}
