using ComprasService.Application.Interfaces;
using ComprasService.Common;


namespace ComprasService.Application.Helpers
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
