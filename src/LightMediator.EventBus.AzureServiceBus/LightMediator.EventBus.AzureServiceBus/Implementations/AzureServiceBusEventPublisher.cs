namespace LightMediator.EventBus.AzureServiceBus.Implementations;

internal class AzureServiceBusEventPublisher : IAzureServiceBusEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public AzureServiceBusEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        return _publishEndpoint.Publish(@event, cancellationToken);
    }
}