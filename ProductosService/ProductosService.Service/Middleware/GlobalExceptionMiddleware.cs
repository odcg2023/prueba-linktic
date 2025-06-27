using ProductosService.Application.Dto.JsonResponse;
using Serilog;
using System.Net;
using System.Text.Json;

namespace ProductosService.Service.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // continua la solicitud
            }
            catch (ApplicationException ex)
            {
                Log.Warning(ex, "Error de validación o negocio");
                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error inesperado no controlado");
                await HandleExceptionAsync(context, "Ha ocurrido un error inesperado", HttpStatusCode.InternalServerError);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, string message, HttpStatusCode statusCode)
        {
            if (context.Response.HasStarted)
            {
                Log.Warning("No se puede modificar la respuesta porque ya fue iniciada.");
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/vnd.api+json"; // JSON API estándar

            var errorResponse = new JsonApiErrorResponse
            {
                Errors = new List<JsonApiError>
            {
                new JsonApiError
                {
                    Status = ((int)statusCode).ToString(),
                    Title = statusCode.ToString(),
                    Detail = message
                }
            },
                Meta = new Meta
                {
                    Success = false,
                    Message = "Error al procesar la solicitud"
                }
            };

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            await context.Response.WriteAsync(json);
        }
    }
}
