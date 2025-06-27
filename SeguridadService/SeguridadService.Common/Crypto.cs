using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static SeguridadService.Common.AppConstants;

namespace SeguridadService.Common
{
    public static class Crypto
    {
        private static byte[] IV = new byte[16];
        public static string Decrypt(string textoEncriptado)
        {
            Aes aes = GetEncryptionAlgorithm();

            byte[] buffer = Convert.FromBase64String(textoEncriptado);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string Encrypt(string texto)
        {
            byte[] array;
            Aes aes = GetEncryptionAlgorithm();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(texto);
                    }

                    array = memoryStream.ToArray();
                }
            }

            return Convert.ToBase64String(array);
        }

        private static Aes GetEncryptionAlgorithm()
        {
            Aes aes = Aes.Create();
            aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(CryptoKeys.KeyCrypto));
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;

            return aes;
        }
    }
}
