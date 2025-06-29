using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

namespace ProductosService.Service.HealthChecks
{
    public class LoggingHealthCheckPublisher : IHealthCheckPublisher
    {
        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            foreach (var entry in report.Entries)
            {
                if (entry.Value.Status != HealthStatus.Healthy)
                {
                    Log.Warning("Health check failed: {CheckName} - {Status} - {Description}",
                        entry.Key,
                        entry.Value.Status,
                        entry.Value.Description ?? "Sin descripción");
                }
            }

            return Task.CompletedTask;
        }
    }
}
