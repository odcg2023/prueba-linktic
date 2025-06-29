﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ComprasService.Application.Exceptions
{
    public class ApplicationException: Exception
    {
        public ApplicationException()
        { }

        public ApplicationException(string message) : base(message)
        { }

        public ApplicationException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
