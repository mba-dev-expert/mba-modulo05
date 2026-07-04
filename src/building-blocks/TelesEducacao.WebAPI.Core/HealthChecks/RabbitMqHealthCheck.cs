using EasyNetQ;
using EasyNetQ.Persistent;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TelesEducacao.WebAPI.Core.HealthChecks;

public class RabbitMqHealthCheck(IBus bus) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await bus.Advanced.EnsureConnectedAsync(PersistentConnectionType.Producer, cancellationToken);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("RabbitMQ desconectado", ex);
        }

        var status = bus.Advanced.GetConnectionStatus(PersistentConnectionType.Producer);

        return status.State == PersistentConnectionState.Connected
            ? HealthCheckResult.Healthy("RabbitMQ conectado")
            : HealthCheckResult.Unhealthy(status.FailureReason ?? "RabbitMQ desconectado");
    }
}
