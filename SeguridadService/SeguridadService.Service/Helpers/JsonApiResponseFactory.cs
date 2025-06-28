using Microsoft.AspNetCore.Mvc;
using ProductosService.Application.Dto.JsonResponse;

namespace SeguridadService.Service.Helpers
{
    public static class JsonApiResponseFactory
    {
        public static IActionResult Success<T>(T data, string type, string message)
        {
            return new OkObjectResult(new JsonApiResponse<T>
            {
                Data = new JsonApiData<T>
                {
                    Type = type,
                    Attributes = data
                },
                Meta = new Meta
                {
                    Success = true,
                    Message = message
                }
            });
        }

        public static IActionResult Error(string title, string detail, string statusCode, string message, int httpStatusCode)
        {
            return new ObjectResult(new JsonApiErrorResponse
            {
                Errors = new List<JsonApiError>
            {
                new JsonApiError
                {
                    Status = statusCode,
                    Title = title,
                    Detail = detail
                }
            },
                Meta = new Meta
                {
                    Success = false,
                    Message = message
                }
            })
            {
                StatusCode = httpStatusCode
            };
        }
    }
}
