using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SeguridadService.Application.Dto;
using SeguridadService.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SeguridadService.Common.AppConstants;

namespace SeguridadService.Service.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public LoggedDto GenerarToken(LoginResponseDto login)
        {
            var keyJwt = Crypto.Decrypt(CryptoKeys.KeyToken);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyJwt));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(signingCredentials);

            var claims = new[]
            {
            new Claim("Expresion1", Crypto.Encrypt(login.Id.ToString())),
            new Claim("Expresion2", Crypto.Encrypt(login.NombreUsuario ?? string.Empty)),
            };

            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Authentication:JwtExpired"]));

            var payload = new JwtPayload(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires
            );

            var token = new JwtSecurityToken(header, payload);

            return new LoggedDto { Token = new JwtSecurityTokenHandler().WriteToken(token) };
        }
    }
}
