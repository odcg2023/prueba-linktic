using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductosService.Application.Dto.JsonResponse
{
    public class JsonApiError
    {
        public string Status { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
    }
}
