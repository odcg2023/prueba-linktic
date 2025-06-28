using SeguridadService.Application.Interfaces;
using SeguridadService.Common;

namespace SeguridadService.Application.Helpers
{
    public class CryptoHelper : ICryptoHelper
    {
        public string Encrypt(string texto)
        {
            return Crypto.Encrypt(texto);
        }

        public string Decrypt(string textoEncriptado)
        {
            return Crypto.Decrypt(textoEncriptado);
        }
    }
}
