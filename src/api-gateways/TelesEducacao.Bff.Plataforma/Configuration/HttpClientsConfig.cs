using Polly;
using TelesEducacao.Bff.Plataforma.Extensions;
using TelesEducacao.Bff.Plataforma.Services;
using TelesEducacao.WebAPI.Core.Extensions;

namespace TelesEducacao.Bff.Plataforma.Configuration;

public static class HttpClientsConfig
{
    public static IServiceCollection AddHttpClientsConfiguration(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

        services.AddHttpClient<IAlunoService, AlunoService>()
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            .AllowSelfSignedCertificateInDevelopment(environment)
            .AddPolicyHandler(PollyExtensions.EsperarTentar())
            .AddTransientHttpErrorPolicy(
                p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        services.AddHttpClient<IAuthService, AuthService>()
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            .AllowSelfSignedCertificateInDevelopment(environment)
            .AddPolicyHandler(PollyExtensions.EsperarTentar())
            .AddTransientHttpErrorPolicy(
                p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        services.AddHttpClient<IConteudoService, ConteudoService>()
            .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
            .AllowSelfSignedCertificateInDevelopment(environment)
            .AddPolicyHandler(PollyExtensions.EsperarTentar())
            .AddTransientHttpErrorPolicy(
                p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        return services;
    }
}
