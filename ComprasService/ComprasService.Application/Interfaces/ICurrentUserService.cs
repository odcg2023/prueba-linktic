﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Interfaces
{
    public interface ICurrentUserService
    {
        short GetCurrentUserId();
        string GetCurrentToken();
    }
}
