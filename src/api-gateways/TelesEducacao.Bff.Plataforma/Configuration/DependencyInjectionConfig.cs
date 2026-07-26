using TelesEducacao.WebAPI.Core.Usuario;

namespace TelesEducacao.Bff.Plataforma.Configuration;

public static class DependencyInjectionConfig
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAspNetUser, AspNetUser>();
    }
}
