using EasyNetQ.Consumer;
using Microsoft.Extensions.Logging;

namespace TelesEducacao.MessageBus.Resilience;

internal sealed class LoggingConsumeErrorStrategy : IConsumeErrorStrategy
{
    private readonly IConsumeErrorStrategy _innerStrategy;
    private readonly ILogger<LoggingConsumeErrorStrategy> _logger;
    private readonly MessageBusResilienceOptions _options;

    public LoggingConsumeErrorStrategy(
        IConsumeErrorStrategy innerStrategy,
        ILogger<LoggingConsumeErrorStrategy> logger,
        MessageBusResilienceOptions options)
    {
        _innerStrategy = innerStrategy;
        _logger = logger;
        _options = options;
    }

    public async ValueTask<AckStrategyAsync> HandleErrorAsync(
        ConsumeContext context,
        Exception exception,
        CancellationToken cancellationToken = default)
    {
        var receivedInfo = context.ReceivedInfo;

        _logger.LogError(
            exception,
            "Consumo da mensagem {MessageType} falhou após {TotalAttempts} tentativa(s); encaminhando para a error queue {ErrorQueue}. Queue={Queue} Exchange={Exchange} RoutingKey={RoutingKey} ConsumerTag={ConsumerTag} DeliveryTag={DeliveryTag} Redelivered={Redelivered} CorrelationId={CorrelationId} MessageId={MessageId}",
            context.Properties.Type,
            _options.TotalAttempts,
            _options.ErrorQueueName,
            receivedInfo.Queue,
            receivedInfo.Exchange,
            receivedInfo.RoutingKey,
            receivedInfo.ConsumerTag,
            receivedInfo.DeliveryTag,
            receivedInfo.Redelivered,
            context.Properties.CorrelationId,
            context.Properties.MessageId);

        var ackStrategy = await _innerStrategy.HandleErrorAsync(context, exception, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation(
            "Mensagem {MessageType} da fila {Queue} encaminhada para a error queue {ErrorQueue} via exchange {ErrorExchange}; consumo prossegue.",
            context.Properties.Type,
            receivedInfo.Queue,
            _options.ErrorQueueName,
            _options.ErrorExchangeName);

        return ackStrategy;
    }

    public ValueTask<AckStrategyAsync> HandleCancelledAsync(
        ConsumeContext context,
        CancellationToken cancellationToken = default)
    {
        var receivedInfo = context.ReceivedInfo;

        _logger.LogWarning(
            "Consumo da mensagem {MessageType} foi cancelado. Queue={Queue} Exchange={Exchange} RoutingKey={RoutingKey} ConsumerTag={ConsumerTag} DeliveryTag={DeliveryTag}",
            context.Properties.Type,
            receivedInfo.Queue,
            receivedInfo.Exchange,
            receivedInfo.RoutingKey,
            receivedInfo.ConsumerTag,
            receivedInfo.DeliveryTag);

        return _innerStrategy.HandleCancelledAsync(context, cancellationToken);
    }
}
