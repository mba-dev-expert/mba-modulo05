using TelesEducacao.Bff.Plataforma.Extensions;
using TelesEducacao.WebAPI.Core.Extensions;
using TelesEducacao.WebAPI.Core.Identidade;

namespace TelesEducacao.Bff.Plataforma.Configuration;

public static class ApiConfig
{
    private const string ServiceName = "bff";

    public static IServiceCollection AddApiConfigurations(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllers();

        services.AddPlatformLogging(configuration, ServiceName);

        services.AddPlatformMetrics(ServiceName);

        services.Configure<AppServicesSettings>(configuration.GetSection("AppServicesSettings"));
        services.AddPlatformCors(configuration, environment);

        services.AddJwtConfiguration(configuration);

        services.AddGatewayHealthChecks(configuration);

        return services;
    }

    public static WebApplication UseApiCoreConfigurations(this WebApplication app)
    {
        app.UsePlatformRequestLogging();

        app.UseCors(CorsExtensions.PolicyName);
        app.UseSwaggerConfiguration();

        app.UseHttpsRedirection();

        app.UseAuthConfiguration();

        app.MapControllers();
        app.MapPlatformHealthChecks();
        app.MapPlatformMetrics();

        return app;
    }
}
