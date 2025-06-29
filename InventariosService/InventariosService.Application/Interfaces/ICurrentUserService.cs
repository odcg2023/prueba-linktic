using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosService.Application.Interfaces
{
    public interface ICurrentUserService
    {
        short GetCurrentUserId();
        string GetCurrentToken();
    }
}
