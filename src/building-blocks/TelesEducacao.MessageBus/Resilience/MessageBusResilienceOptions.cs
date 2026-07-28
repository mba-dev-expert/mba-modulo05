using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace TelesEducacao.MessageBus.Resilience;

public sealed class MessageBusResilienceOptions
{
    public const string SectionName = "MessageBusResilience";

    private static readonly TimeSpan[] DefaultRetryDelays =
    [
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10)
    ];

    public ushort PrefetchCount { get; set; } = 10;

    public int MaxRetryAttempts { get; set; } = 3;

    public IReadOnlyList<TimeSpan> RetryDelays { get; set; } = DefaultRetryDelays;

    public string ErrorQueueName { get; set; } = "TelesEducacao_Error_Queue";

    public string ErrorExchangeName { get; set; } = "TelesEducacao_Error_Exchange";

    public int TotalAttempts => MaxRetryAttempts + 1;

    public TimeSpan GetRetryDelay(int attempt)
    {
        if (RetryDelays.Count == 0)
        {
            return TimeSpan.Zero;
        }

        // Se houver menos delays que tentativas, o último valor é reutilizado.
        var index = Math.Min(Math.Max(attempt, 1), RetryDelays.Count) - 1;
        return RetryDelays[index];
    }

    public static MessageBusResilienceOptions FromConfiguration(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new MessageBusResilienceOptions();
        var section = configuration.GetSection(SectionName);

        if (!section.Exists())
        {
            return options;
        }

        if (ushort.TryParse(section["PrefetchCount"], NumberStyles.Integer, CultureInfo.InvariantCulture, out var prefetchCount)
            && prefetchCount > 0)
        {
            options.PrefetchCount = prefetchCount;
        }

        if (int.TryParse(section["MaxRetryAttempts"], NumberStyles.Integer, CultureInfo.InvariantCulture, out var maxRetryAttempts)
            && maxRetryAttempts >= 0)
        {
            options.MaxRetryAttempts = maxRetryAttempts;
        }

        var retryDelays = ReadRetryDelays(section);
        if (retryDelays.Count > 0)
        {
            options.RetryDelays = retryDelays;
        }

        var errorQueueName = section["ErrorQueueName"];
        if (!string.IsNullOrWhiteSpace(errorQueueName))
        {
            options.ErrorQueueName = errorQueueName;
        }

        var errorExchangeName = section["ErrorExchangeName"];
        if (!string.IsNullOrWhiteSpace(errorExchangeName))
        {
            options.ErrorExchangeName = errorExchangeName;
        }

        return options;
    }

    private static IReadOnlyList<TimeSpan> ReadRetryDelays(IConfiguration section)
    {
        var delays = new List<TimeSpan>();

        foreach (var child in section.GetSection("RetryDelaysInSeconds").GetChildren())
        {
            if (double.TryParse(child.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds)
                && seconds >= 0)
            {
                delays.Add(TimeSpan.FromSeconds(seconds));
            }
        }

        return delays;
    }
}
