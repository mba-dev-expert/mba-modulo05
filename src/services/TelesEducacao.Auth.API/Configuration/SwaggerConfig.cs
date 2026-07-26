using Microsoft.OpenApi.Models;

namespace TelesEducacao.Auth.API.Configuration;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerConfigureServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Teles Educação API de Autenticação",
                Version = "v1",
                Description = "Documentação da API de autenticação JWT"
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Teles Educação API Autenticação v1");
        });

        return app;
    }
}
