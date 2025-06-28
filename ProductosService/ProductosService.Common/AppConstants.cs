using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductosService.Common
{
    public static class AppConstants
    {
        public static class CryptoKeys
        {
            public const string KeyCrypto = "h89Mz&Au6$#0^2K"; //Solo para la prueba técnica, esto debe ir como variable de entorno o en Azure Key Vault, AWS Secrets, etc
            public const string KeyToken = "WTDQDgB2oqhtkhG+RVQh2TCSS7CiSHq14TK7Rvv5on4s5ALCgPTH3c7Ua7AlAvJY"; //VALOR ENCRIPTADO - Solo para la prueba técnica, esto debe ir como variable de entorno o en Azure Key Vault, AWS Secrets, etc
        }

        public static class Messages
        {
            public const string PeticionCorrecta = "Petición ejecutada de forma correcta";
            public const string ProductoInexistente = "El producto consultado no existe";
        }
    }
}
