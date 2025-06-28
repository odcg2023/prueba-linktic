using ProductosService.Application.Interfaces;
using ProductosService.Common;


namespace ProductosService.Application.Helpers
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
