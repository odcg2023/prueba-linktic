using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductosService.Application.Dto.JsonResponse
{
    public class JsonApiResponse<T>
    {
        public JsonApiData<T> Data { get; set; }
        public Meta Meta { get; set; }
    }
}
