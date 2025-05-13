 namespace LightMediator.EventBus.SignalR;

internal class SignalREventBus : ILightMediatorEventBus, IHostedService
{
    private readonly IMediator _mediator;
    private readonly HubConnection _hubConnection;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<SignalREventBus> _logger;

    public SignalREventBus(
        IMediator mediator,
        HubConnection hubConnection,
        IHostApplicationLifetime lifetime,
        ILogger<SignalREventBus> logger)
    {
        _mediator = mediator;
        _hubConnection = hubConnection;
        _lifetime = lifetime;
        _logger = logger;
    }

    public async Task OnEventRecieved(string notificationMessage, CancellationToken? cancellationToken = null)
    {
        try
        {
            var signalREvent = JsonConvert.DeserializeObject<SignalREvent>(notificationMessage)
                               ?? throw new EventDeserializationException("Failed to deserialize SignalREvent.");

            var type = Type.GetType(signalREvent.TypeName);
            if (type == null)
                throw new EventDeserializationException($"Unable to resolve type: {signalREvent.TypeName}");

            var notification = (INotification?)JsonConvert.DeserializeObject(signalREvent.JsonPayload, type);
            if (notification == null)
                throw new EventDeserializationException("Failed to deserialize payload to INotification.");

            await _mediator.Publish(notification, cancellationToken ?? CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process SignalR event.");
            throw;
        }
    }

    public async Task PublishAsync(INotification notification)
    {
        try
        {
            if (_hubConnection.ConnectionId == null)
                throw new SignalRConnectionException("SignalR connection not established.");

            var eventMessage = new SignalREvent(
                notification.GetType().AssemblyQualifiedName!,
                JsonConvert.SerializeObject(notification)
            );

            var serializedMessage = JsonConvert.SerializeObject(eventMessage);

            await _hubConnection.InvokeAsync("NewEventRaised", serializedMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish SignalR event.");
            throw;
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lifetime.ApplicationStarted.Register(async () =>
        {
            try
            {
                await HandleSignalRMessages(cancellationToken);

                int retries = 0;
                while (_hubConnection.ConnectionId == null && retries < 5)
                {
                    try
                    {
                        await _hubConnection.StartAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Retrying SignalR connection...");
                    }

                    if (_hubConnection.ConnectionId == null)
                    {
                        retries++;
                        await Task.Delay(3000, cancellationToken);
                    }
                }

                if (_hubConnection.ConnectionId == null)
                {
                    throw new SignalRConnectionException("Failed to establish SignalR connection after retries.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "SignalREventBus failed to start.");
                throw;
            }
        });

        return Task.CompletedTask;
    }

    private Task HandleSignalRMessages(CancellationToken? cancellationToken = null)
    {
        _hubConnection.On<string>("EventRecieved", async message =>
        {
            try
            {
                await OnEventRecieved(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling incoming SignalR message.");
            }
        });

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _hubConnection.StopAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop SignalR connection cleanly.");
        }
    }
}
