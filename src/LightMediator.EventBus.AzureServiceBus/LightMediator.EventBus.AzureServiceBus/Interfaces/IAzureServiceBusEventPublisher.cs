namespace LightMediator.EventBus.AzureServiceBus.Interfaces;

internal interface IAzureServiceBusEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;
}
