using EasyNetQ.Consumer;

namespace TelesEducacao.MessageBus.Resilience;

public static class ConsumePipelineBuilderExtensions
{
    // Deve vir depois de UseConsumeErrorStrategy para que a exceção final ainda caia na error queue.
    public static ConsumePipelineBuilder UseConsumeRetry(
        this ConsumePipelineBuilder pipelineBuilder,
        MessageBusResilienceOptions options)
    {
        ArgumentNullException.ThrowIfNull(pipelineBuilder);
        ArgumentNullException.ThrowIfNull(options);

        return pipelineBuilder.Use(next => new ConsumeRetryMiddleware(next, options).InvokeAsync);
    }
}
