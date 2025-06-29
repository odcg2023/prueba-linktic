using Microsoft.AspNetCore.Mvc;
using ComprasService.Application.Dto.JsonResponse;

namespace ComprasService.Service.Helpers
{
    public static class JsonApiResponseFactory
    {
        public static IActionResult Success<T>(T data, string type, string message)
        {
            var response = new JsonApiResponse<T>
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
            };

            return new OkObjectResult(response);
        }

        public static IActionResult NotFound(string detail, string message = "Recurso no encontrado")
        {
            return BuildErrorResponse("404", "NotFound", detail, message, StatusCodes.Status404NotFound);
        }

        public static IActionResult BadRequest(string detail, string message = "Solicitud inválida")
        {
            return BuildErrorResponse("400", "BadRequest", detail, message, StatusCodes.Status400BadRequest);
        }

        public static IActionResult InternalServerError(string detail, string message = "Ocurrió un error inesperado")
        {
            return BuildErrorResponse("500", "InternalServerError", detail, message, StatusCodes.Status500InternalServerError);
        }

        private static IActionResult BuildErrorResponse(string status, string title, string detail, string message, int httpStatusCode)
        {
            var errorResponse = new JsonApiErrorResponse
            {
                Errors = new List<JsonApiError>
            {
                new JsonApiError
                {
                    Status = status,
                    Title = title,
                    Detail = detail
                }
            },
                Meta = new Meta
                {
                    Success = false,
                    Message = message
                }
            };

            return new ObjectResult(errorResponse)
            {
                StatusCode = httpStatusCode
            };
        }
    }

}
