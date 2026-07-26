using TelesEducacao.Bff.Plataforma.Extensions;
using TelesEducacao.WebAPI.Core.Extensions;
using TelesEducacao.WebAPI.Core.Identidade;

namespace TelesEducacao.Bff.Plataforma.Configuration;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.Configure<AppServicesSettings>(configuration.GetSection("AppServicesSettings"));
        services.AddCors(options =>
        {
            options.AddPolicy("Total",
                builder =>
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
        });

        services.AddJwtConfiguration(configuration);

        services.AddHealthChecks();

        return services;
    }

    public static WebApplication UseApiCoreConfigurations(this WebApplication app)
    {
        app.UseCors("Total");
        app.UseSwaggerConfiguration();

        app.UseHttpsRedirection();

        app.UseAuthConfiguration();

        app.MapControllers();
        app.MapPlatformHealthChecks();

        return app;
    }
}
