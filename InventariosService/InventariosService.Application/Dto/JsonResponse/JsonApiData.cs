using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosService.Application.Dto.JsonResponse
{
    public class JsonApiData<T>
    {
        public string Type { get; set; }
        public T Attributes { get; set; }
    }
}
