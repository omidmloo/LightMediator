namespace LightMediator.EventBus.RabbitMQ;

internal interface IRabbitMqEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;
}
