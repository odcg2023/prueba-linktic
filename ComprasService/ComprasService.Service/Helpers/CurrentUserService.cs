using ComprasService.Application.Interfaces;

namespace ComprasService.Service.Helpers
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
    }
}
