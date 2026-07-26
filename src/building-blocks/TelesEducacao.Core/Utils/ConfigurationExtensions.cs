using Microsoft.Extensions.Configuration;

namespace TelesEducacao.Core.Utils;

public static class ConfigurationExtensions
{
    public static string GetMessageQueueConnection(this IConfiguration configuration, string name)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return configuration.GetSection("MessageQueueConnection")[name]
               ?? throw new InvalidOperationException(
                   $"A connection string '{name}' não foi encontrada na seção 'MessageQueueConnection'.");
    }
}