using EasyNetQ.Consumer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TelesEducacao.MessageBus.Resilience;

internal sealed class ConsumeRetryMiddleware
{
    private readonly ConsumeDelegate _next;
    private readonly MessageBusResilienceOptions _options;

    public ConsumeRetryMiddleware(ConsumeDelegate next, MessageBusResilienceOptions options)
    {
        _next = next;
        _options = options;
    }

    public async ValueTask<AckStrategyAsync> InvokeAsync(ConsumeContext context)
    {
        var logger = context.Services.GetRequiredService<ILogger<ConsumeRetryMiddleware>>();
        var totalAttempts = _options.TotalAttempts;

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await _next(context).ConfigureAwait(false);
            }
            // Esgotadas as tentativas, a exceção sobe para a IConsumeErrorStrategy.
            catch (Exception exception) when (attempt < totalAttempts && !IsConsumerShutdown(exception, context))
            {
                var delay = _options.GetRetryDelay(attempt);
                var receivedInfo = context.ReceivedInfo;

                logger.LogWarning(
                    exception,
                    "Falha no consumo da mensagem {MessageType} (tentativa {Attempt} de {TotalAttempts}). Nova tentativa em {RetryDelaySeconds}s. Queue={Queue} Exchange={Exchange} RoutingKey={RoutingKey} ConsumerTag={ConsumerTag} DeliveryTag={DeliveryTag} CorrelationId={CorrelationId}",
                    context.Properties.Type,
                    attempt,
                    totalAttempts,
                    delay.TotalSeconds,
                    receivedInfo.Queue,
                    receivedInfo.Exchange,
                    receivedInfo.RoutingKey,
                    receivedInfo.ConsumerTag,
                    receivedInfo.DeliveryTag,
                    context.Properties.CorrelationId);

                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, context.CancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    private static bool IsConsumerShutdown(Exception exception, ConsumeContext context)
        => exception is OperationCanceledException && context.CancellationToken.IsCancellationRequested;
}
