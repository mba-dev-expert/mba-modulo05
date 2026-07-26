using MediatR;
using Microsoft.AspNetCore.Identity;
using NetDevPack.Security.JwtSigningCredentials;
using NetDevPack.Security.JwtSigningCredentials.AspNetCore;
using System.Reflection;
using TelesEducacao.Auth.Application.Extensions;
using TelesEducacao.Auth.Data;
using TelesEducacao.WebAPI.Core.Data;
using TelesEducacao.WebAPI.Core.Extensions;

namespace TelesEducacao.Auth.API.Configuration;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfigurations(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Services
        services.AddControllers();

        // Database
        services.AddDatabase<AuthDbContext>(configuration, environment);

        // Identity
        var appSettingsSection = configuration.GetSection("AppTokenSettings");
        services.Configure<AppTokenSettings>(appSettingsSection);

        services.AddJwksManager(options => options.Algorithm = Algorithm.ES256)
            .PersistKeysToDatabaseStore<AuthDbContext>();

        services.AddMessageBusConfiguration(configuration);

        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedEmail = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddMemoryCache();

        services.AddPlatformHealthChecks(configuration, environment);

        return services;
    }

    public static void UseApiCoreConfigurations(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseJwksDiscovery();
        app.MapPlatformHealthChecks();
    }
}
