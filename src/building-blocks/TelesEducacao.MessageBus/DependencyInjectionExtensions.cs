using EasyNetQ;
using EasyNetQ.ConnectionString;
using EasyNetQ.Consumer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TelesEducacao.Core.Utils;
using TelesEducacao.MessageBus.Resilience;

namespace TelesEducacao.MessageBus;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return services.AddMessageBus(
            configuration.GetMessageQueueConnection("MessageBus"),
            MessageBusResilienceOptions.FromConfiguration(configuration));
    }

    public static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        string connectionString,
        MessageBusResilienceOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ArgumentNullException.ThrowIfNull(options);

        services.AddSingleton(options);

        // Error queue/exchange explícitos, em vez do "EasyNetQ_Default_Error_Queue" implícito.
        services.AddSingleton<IConventions>(serviceProvider =>
            new Conventions(serviceProvider.GetRequiredService<ITypeNameSerializer>())
            {
                ErrorQueueNamingConvention = _ => options.ErrorQueueName,
                ErrorExchangeNamingConvention = _ => options.ErrorExchangeName
            });

        // Com o retry por dentro da error strategy, a exceção só chega nela depois de
        // esgotadas as tentativas — é aí que a mensagem vai para a error queue.
        services.AddSingleton(_ => new ConsumePipelineBuilder()
            .UseConsumeErrorStrategy()
            .UseConsumeInterceptors()
            .UseConsumeRetry(options));

        services.AddSingleton<IConsumeErrorStrategy>(serviceProvider => new LoggingConsumeErrorStrategy(
            ActivatorUtilities.CreateInstance<DefaultConsumeErrorStrategy>(serviceProvider),
            serviceProvider.GetRequiredService<ILogger<LoggingConsumeErrorStrategy>>(),
            options));

        services.AddEasyNetQ(serviceProvider =>
        {
            var connectionConfiguration = serviceProvider
                .GetRequiredService<IConnectionStringParser>()
                .Parse(connectionString);

            connectionConfiguration.PrefetchCount = options.PrefetchCount;

            return connectionConfiguration;
        });

        services.AddSingleton<IMessageBus, MessageBus>();

        return services;
    }
}
