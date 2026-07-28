using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using TelesEducacao.WebAPI.Core.HealthChecks;

namespace TelesEducacao.WebAPI.Core.Extensions;

public static class HealthCheckExtensions
{
    public const string ReadyTag = "ready";

    private const string AppServicesSettingsSection = "AppServicesSettings";
    private static readonly TimeSpan GatewayHealthCheckTimeout = TimeSpan.FromSeconds(5);

    // Readiness dos gateways (BFFs), que não têm banco nem barramento próprios: um check por
    // serviço downstream, apontando para o "/health/live" do destino para não cascatear readiness.
    public static IServiceCollection AddGatewayHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var healthChecks = services.AddHealthChecks();
        var appServicesSection = configuration.GetSection(AppServicesSettingsSection);

        AddDownstreamHealthCheck(healthChecks, appServicesSection["AuthUrl"], "auth");
        AddDownstreamHealthCheck(healthChecks, appServicesSection["AlunoUrl"], "alunos");
        AddDownstreamHealthCheck(healthChecks, appServicesSection["ConteudoUrl"], "conteudo");

        return services;
    }

    private static void AddDownstreamHealthCheck(IHealthChecksBuilder healthChecks, string? baseUrl, string name)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            return;

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri))
            return;

        var livenessUri = new Uri(baseUri, "/health/live");

        healthChecks.AddUrlGroup(livenessUri, name: name, tags: [ReadyTag], timeout: GatewayHealthCheckTimeout);
    }

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
