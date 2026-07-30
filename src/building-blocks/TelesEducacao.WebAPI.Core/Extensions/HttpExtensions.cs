using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TelesEducacao.WebAPI.Core.Extensions;

public static class HttpExtensions
{
    public static IHttpClientBuilder AllowSelfSignedCertificateInDevelopment(
        this IHttpClientBuilder builder,
        IWebHostEnvironment environment)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (environment == null) throw new ArgumentNullException(nameof(environment));

        if (!environment.IsDevelopment())
            return builder;

        return builder.ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
        });
    }
}
