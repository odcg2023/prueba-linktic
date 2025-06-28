using Microsoft.AspNetCore.Mvc;
using ProductosService.Application.Dto.JsonResponse;
using SeguridadService.Service.Helpers;
using Serilog;
using System.Net;
using System.Text.Json;

namespace SeguridadService.Service.Middleware
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
                await _next(context);
            }
            catch (ApplicationException ex)
            {
                Log.Warning(ex, "Error de validación o negocio");

                var result = JsonApiResponseFactory.Error(
                    title: "Bad Request",
                    detail: ex.Message,
                    statusCode: "400",
                    message: "Error al procesar la solicitud. Valide los valores enviados.",
                    httpStatusCode: StatusCodes.Status400BadRequest
                );

                await WriteResultAsync(context, result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error inesperado no controlado");

                var result = JsonApiResponseFactory.Error(
                    title: "Internal Server Error",
                    detail: "Ha ocurrido un error inesperado.",
                    statusCode: "500",
                    message: "Error al procesar la solicitud.",
                    httpStatusCode: StatusCodes.Status500InternalServerError
                );

                await WriteResultAsync(context, result);
            }
        }

        private async Task WriteResultAsync(HttpContext context, IActionResult actionResult)
        {
            if (context.Response.HasStarted)
            {
                Log.Warning("No se puede modificar la respuesta porque ya fue iniciada.");
                return;
            }

            context.Response.Clear();

            if (actionResult is ObjectResult objectResult)
            {
                context.Response.StatusCode = objectResult.StatusCode ?? StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/vnd.api+json";

                var json = JsonSerializer.Serialize(objectResult.Value, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });

                await context.Response.WriteAsync(json);
            }
        }
    }
}
