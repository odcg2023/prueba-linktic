using InventariosService.Application.Interfaces;
using InventariosService.Common;


namespace InventariosService.Application.Helpers
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
