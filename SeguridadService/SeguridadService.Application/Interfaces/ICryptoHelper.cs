using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeguridadService.Application.Interfaces
{
    public interface ICryptoHelper
    {
        string Encrypt(string texto);
        string Decrypt(string textoEncriptado);
    }
}
