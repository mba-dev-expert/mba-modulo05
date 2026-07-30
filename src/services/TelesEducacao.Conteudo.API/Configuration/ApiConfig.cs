using TelesEducacao.Conteudo.Application.AutoMapper;
using TelesEducacao.Conteudo.Data;
using TelesEducacao.WebAPI.Core.Data;
using TelesEducacao.WebAPI.Core.Extensions;
using TelesEducacao.WebAPI.Core.Identidade;

namespace TelesEducacao.Conteudo.API.Configuration;

public static class ApiConfig
{
    private const string ServiceName = "conteudo";

    public static IServiceCollection AddApiConfigurations(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllers();

        services.AddPlatformLogging(configuration, ServiceName);

        services.AddPlatformMetrics(ServiceName);

        services.AddDatabase<ConteudosContext>(configuration, environment);

        services.AddPlatformCors(configuration, environment);

        services.AddAutoMapper(cfg => { }, typeof(DtoToDomainMappingProfile),
            typeof(DomainToDtoMappingProfile));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });

        services.AddMessageBusConfiguration(configuration);

        services.AddJwtConfiguration(configuration);

        services.AddPlatformHealthChecks(configuration, environment);

        return services;
    }

    public static void UseApiCoreConfigurations(this WebApplication app)
    {
        app.UsePlatformRequestLogging();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseRouting();

        app.UseCors(CorsExtensions.PolicyName);

        app.UseAuthConfiguration();

        app.MapControllers();
        app.MapPlatformHealthChecks();
        app.MapPlatformMetrics();
    }

}
