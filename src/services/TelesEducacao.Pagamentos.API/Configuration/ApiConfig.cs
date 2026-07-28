using MediatR;
using TelesEducacao.Pagamentos.API.Controllers;
using TelesEducacao.Pagamentos.Business;
using TelesEducacao.Pagamentos.Data;
using TelesEducacao.WebAPI.Core.Data;
using TelesEducacao.WebAPI.Core.Extensions;
using TelesEducacao.WebAPI.Core.Identidade;

namespace TelesEducacao.Pagamentos.API.Configuration;

public static class ApiConfig
{
    private const string ServiceName = "pagamentos";

    public static IServiceCollection AddApiConfigurations(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllers();

        services.AddPlatformLogging(configuration, ServiceName);

        services.AddPlatformMetrics(ServiceName);

        services.AddDatabase<PagamentosContext>(configuration, environment);

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(
                typeof(PagamentosController).Assembly, // API
                typeof(PagamentoService).Assembly,     // Business
                typeof(PagamentosContext).Assembly     // Data
            ));

        services.AddJwtConfiguration(configuration);

        services.AddMessageBusConfiguration(configuration);

        services.AddPlatformHealthChecks(configuration, environment);

        services.AddAuthorization();

        return services;
    }

    public static void UseApiCoreConfigurations(this WebApplication app)
    {
        app.UsePlatformRequestLogging();

        app.UseHttpsRedirection();

        app.UseAuthConfiguration();

        app.MapControllers();
        app.MapPlatformHealthChecks();
        app.MapPlatformMetrics();
    }
}
