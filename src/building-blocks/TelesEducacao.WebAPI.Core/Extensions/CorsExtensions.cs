using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TelesEducacao.WebAPI.Core.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "Platform";

    private const string CorsSettingsSection = "CorsSettings";

    public static IServiceCollection AddPlatformCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var allowedOrigins = configuration.GetSection(CorsSettingsSection).Get<CorsSettings>()?.AllowedOrigins
            ?? [];

        if (!environment.IsDevelopment() && allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException(
                $"Nenhuma origem configurada em \"{CorsSettingsSection}:{nameof(CorsSettings.AllowedOrigins)}\" " +
                $"para o ambiente \"{environment.EnvironmentName}\". Configure ao menos uma origem permitida " +
                "antes de subir a aplicação fora de Development.");
        }

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (environment.IsDevelopment())
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                }
                else
                {
                    policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
                }
            });
        });

        return services;
    }
}
