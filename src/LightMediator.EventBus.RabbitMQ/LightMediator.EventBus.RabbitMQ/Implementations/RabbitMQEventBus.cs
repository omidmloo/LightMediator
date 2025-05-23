using MassTransit;
using System.Threading;

namespace LightMediator.EventBus.RabbitMQ;

internal class RabbitMQEventBus<TEvent> : ILightMediatorEventBus, IConsumer<TEvent> where TEvent : RabbitMQEvent
{
    private readonly IMediator _mediator;
    private readonly ILogger<RabbitMQEventBus<TEvent>> _logger;
    internal readonly LightMediatorOptions _mediatorOptions;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQEventBus(
        IMediator mediator,
        ILogger<RabbitMQEventBus<TEvent>> logger,
        IServiceProvider serviceProvider,
        LightMediatorOptions mediatorOptions)
    {
        _mediator = mediator;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _mediatorOptions = mediatorOptions;
    }

    public async Task Consume(ConsumeContext<TEvent> context)
    { 
        try
        {
            var textMessage = JsonConvert.SerializeObject(context.Message);
            var rabbitMQEvent = JsonConvert.DeserializeObject<RabbitMQEvent>(textMessage)
                                   ?? throw new EventDeserializationException("Failed to deserialize RabbitMQEvent.");

            var currentAssembly = Assembly.GetEntryAssembly()?.FullName;
            if (rabbitMQEvent.AssemblyName == currentAssembly)
            {
                await context.Publish(context.Message);
            }
            else
            {
                await OnEventRecieved(textMessage, null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process RabbitMQ event.");
            throw;
        }
    }

    public async Task OnEventRecieved(string notificationMessage, CancellationToken? cancellationToken)
    {
        try
        {
            var rabbitMQEvent = JsonConvert.DeserializeObject<RabbitMQEvent>(notificationMessage)
                               ?? throw new EventDeserializationException("Failed to deserialize RabbitMQEvent.");
            var currentAssembly = Assembly.GetEntryAssembly()?.FullName;
            if (rabbitMQEvent.AssemblyName == currentAssembly)
                return;
            var t = _mediatorOptions.Assemblies.SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(rabbitMQEvent.TypeName, StringComparison.Ordinal));
            if (t == null)
                throw new EventDeserializationException("The type not found in referenced assemblies");
            var notification = (INotification?)JsonConvert.DeserializeObject(rabbitMQEvent.JsonPayload, t);
            if (notification == null)
                throw new EventDeserializationException("Failed to deserialize payload to INotification.");

            await _mediator.Publish(notification, cancellationToken ?? CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process RabbitMQ event.");
            throw;
        }
    }

    public async Task PublishAsync(INotification notification)
    {
        using var scope = _serviceProvider.CreateScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IRabbitMqEventPublisher>();
        var eventMessage = new RabbitMQEvent(
            notification.GetType().Name.Split(".").Last(),
            JsonConvert.SerializeObject(notification),
            Assembly.GetEntryAssembly().FullName
        );

        await publisher.PublishAsync(eventMessage);
    }

}
