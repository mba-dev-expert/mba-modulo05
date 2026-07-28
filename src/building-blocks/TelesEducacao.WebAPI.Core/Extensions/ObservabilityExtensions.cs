using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace TelesEducacao.WebAPI.Core.Extensions;

public static class ObservabilityExtensions
{
    private const string MetricsEndpoint = "/metrics";
    private const string HealthEndpointPrefix = "/health";
    private const string ServicePropertyName = "Service";

    public static IServiceCollection AddPlatformLogging(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.AddSerilog((serviceProvider, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(serviceProvider)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty(ServicePropertyName, serviceName)
            .WriteTo.Console(new CompactJsonFormatter()));

        return services;
    }

    public static IServiceCollection AddPlatformMetrics(this IServiceCollection services, string serviceName)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName: serviceName))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter());

        return services;
    }

    public static void UsePlatformRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.GetLevel = (httpContext, _, exception) =>
            {
                if (IsObservabilityEndpoint(httpContext.Request.Path))
                    return LogEventLevel.Verbose;

                return exception is not null || httpContext.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : LogEventLevel.Information;
            };
        });
    }

    public static void MapPlatformMetrics(this WebApplication app)
    {
        app.MapPrometheusScrapingEndpoint().AllowAnonymous();
    }

    private static bool IsObservabilityEndpoint(PathString path) =>
        path.StartsWithSegments(HealthEndpointPrefix, StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments(MetricsEndpoint, StringComparison.OrdinalIgnoreCase);
}
