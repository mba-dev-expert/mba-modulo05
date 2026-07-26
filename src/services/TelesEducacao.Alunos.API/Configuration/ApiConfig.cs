using MediatR;
using System.Reflection;
using TelesEducacao.Alunos.Application.AutoMapper;
using TelesEducacao.Alunos.Data;
using TelesEducacao.WebAPI.Core.Data;
using TelesEducacao.WebAPI.Core.Extensions;
using TelesEducacao.WebAPI.Core.Identidade;

namespace TelesEducacao.Alunos.API.Configuration;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfigurations(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllers();

        services.AddDatabase<AlunosContext>(configuration, environment);

        services.AddAutoMapper(cfg => { },
            typeof(AlunosDtoToDomainMappingProfile),
            typeof(AlunosDomainToDtoMappingProfile));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddJwtConfiguration(configuration);

        services.AddMessageBusConfiguration(configuration);

        services.AddPlatformHealthChecks(configuration, environment);

        return services;
    }

    public static void UseApiCoreConfigurations(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseAuthConfiguration();

        app.MapControllers();
        app.MapPlatformHealthChecks();
    }
}
