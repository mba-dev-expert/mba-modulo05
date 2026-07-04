using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelesEducacao.WebAPI.Core.HealthChecks;

namespace TelesEducacao.WebAPI.Core.Extensions;

public static class HealthCheckExtensions
{
    public const string ReadyTag = "ready";

    public static IServiceCollection AddPlatformHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var healthChecks = services.AddHealthChecks();

        if (!environment.IsDevelopment())
        {
            var connection = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connection))
                connection = configuration.GetConnectionString("SqlServer");

            if (!string.IsNullOrEmpty(connection))
                healthChecks.AddSqlServer(connection, name: "sqlserver", tags: [ReadyTag]);
        }

        healthChecks.AddCheck<RabbitMqHealthCheck>("rabbitmq", tags: [ReadyTag]);

        return services;
    }

    public static void MapPlatformHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(ReadyTag)
        });

        app.MapHealthChecks("/health");
    }
}
