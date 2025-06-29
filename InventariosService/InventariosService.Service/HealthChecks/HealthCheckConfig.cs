using HealthChecks.UI.Client;
using InventariosService.Service.Helpers;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InventariosService.Service.HealthChecks
{
    public static class HealthCheckConfig
    {
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: configuration.GetSecureConnectionString("DbPruebaTecnica"),
                    name: "Base de datos",
                    healthQuery: "SELECT 1",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
                    tags: new[] { "Infraestructura" });

            return services;
        }

        public static IEndpointRouteBuilder UseAppHealthCheckEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return endpoints;
        }
    }
}
