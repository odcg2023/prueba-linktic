using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComprasService.Application.Dto.JsonResponse
{
    public class JsonApiErrorResponse
    {
        public List<JsonApiError> Errors { get; set; }
        public Meta Meta { get; set; }
    }
}
