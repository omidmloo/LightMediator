namespace LightMediator.EventBus.RabbitMQ;

public class RabbitMqEventPublisher : IRabbitMqEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public RabbitMqEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
         return _publishEndpoint.Publish(@event, cancellationToken); 
    }
}