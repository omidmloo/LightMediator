namespace LightMediator.EventBus.AzureServiceBus;

internal class AzureServiceBusEventBus<TEvent> : ILightMediatorEventBus, IConsumer<TEvent> where TEvent : AzureServiceBusEvent
{
    private readonly IMediator _mediator;
    private readonly ILogger<AzureServiceBusEventBus<TEvent>> _logger;
    internal readonly LightMediatorOptions _mediatorOptions;
    private readonly IServiceProvider _serviceProvider;

    public AzureServiceBusEventBus(
        IMediator mediator,
        ILogger<AzureServiceBusEventBus<TEvent>> logger,
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
            var azureServiceBusEvent = JsonConvert.DeserializeObject<AzureServiceBusEvent>(textMessage)
                                   ?? throw new EventDeserializationException("Failed to deserialize AzureServiceBusEvent.");

            var currentAssembly = Assembly.GetEntryAssembly()?.FullName;
            if (azureServiceBusEvent.AssemblyName != currentAssembly)
            {
                await OnEventRecieved(textMessage, null);
            } 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process AzureServiceBus event.");
            throw;
        }
    }

    public async Task OnEventRecieved(string notificationMessage, CancellationToken? cancellationToken)
    {
        try
        {
            var azureServiceBusEvent = JsonConvert.DeserializeObject<AzureServiceBusEvent>(notificationMessage)
                               ?? throw new EventDeserializationException("Failed to deserialize AzureServiceBusEvent.");
            var currentAssembly = Assembly.GetEntryAssembly()?.FullName;
            if (azureServiceBusEvent.AssemblyName == currentAssembly)
                return;
            var t = _mediatorOptions.Assemblies.SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(azureServiceBusEvent.TypeName, StringComparison.Ordinal));
            if (t == null)
                throw new EventDeserializationException("The type not found in referenced assemblies");
            var notification = (INotification?)JsonConvert.DeserializeObject(azureServiceBusEvent.JsonPayload, t);
            if (notification == null)
                throw new EventDeserializationException("Failed to deserialize payload to INotification.");

            await _mediator.Publish(notification, cancellationToken ?? CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process AzureServiceBus event.");
            throw;
        }
    }

    public async Task PublishAsync(INotification notification)
    {
        using var scope = _serviceProvider.CreateScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IAzureServiceBusEventPublisher>();
        var eventMessage = new AzureServiceBusEvent(
            notification.GetType().Name.Split(".").Last(),
            JsonConvert.SerializeObject(notification),
            Assembly.GetEntryAssembly()!.FullName!
        );

        await publisher.PublishAsync(eventMessage);
    }

}
