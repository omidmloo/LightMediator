 namespace LightMediator.EventBus.SignalR;

internal class SignalREventBus : ILightMediatorEventBus, IHostedService
{
    private readonly IMediator _mediator;
    private readonly HubConnection _hubConnection;
    private readonly IHostApplicationLifetime _lifetime;
    internal readonly LightMediatorOptions _mediatorOptions;
    private readonly ILogger<SignalREventBus> _logger;

    public SignalREventBus(
        IMediator mediator,
        HubConnection hubConnection,
        IHostApplicationLifetime lifetime,
        ILogger<SignalREventBus> logger,
        LightMediatorOptions mediatorOptions)
    {
        _mediator = mediator;
        _hubConnection = hubConnection;
        _lifetime = lifetime;
        _logger = logger;

        RegisterConnectionHandlers();
        _mediatorOptions = mediatorOptions;
    }

    private void RegisterConnectionHandlers()
    {
        _hubConnection.Closed += async (error) =>
        {
            _logger.LogWarning(error, "SignalR connection closed. Attempting to reconnect...");

            while (true)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    _logger.LogInformation("SignalR reconnected successfully after disconnect.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Reconnection failed. Retrying in 5 seconds...");
                    await Task.Delay(5000);
                }
            }
        };

        _hubConnection.Reconnecting += error =>
        {
            _logger.LogWarning(error, "SignalR connection lost. Reconnecting...");
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectionId =>
        {
            _logger.LogInformation("SignalR reconnected. ConnectionId: {ConnectionId}", connectionId);
            return Task.CompletedTask;
        };
    }

    public async Task OnEventRecieved(string notificationMessage, CancellationToken? cancellationToken = null)
    {
        try
        {
            var signalREvent = JsonConvert.DeserializeObject<SignalREvent>(notificationMessage)
                               ?? throw new EventDeserializationException("Failed to deserialize SignalREvent.");

            var t = _mediatorOptions.Assemblies.SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith(signalREvent.TypeName, StringComparison.Ordinal));
            if (t == null)
                throw new EventDeserializationException("The type not found in referenced assemblies");
            var notification = (INotification?)JsonConvert.DeserializeObject(signalREvent.JsonPayload, t); 
            if (notification == null)
                throw new EventDeserializationException("Failed to deserialize payload to INotification.");

            await _mediator.Publish(notification, cancellationToken ?? CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process SignalR event."); 
        }
    }


    public Task PublishAsync(INotification notification)
    {
        if (_hubConnection.ConnectionId != null)
        {
            return SendNotificationAsync(notification);
        }

        // Run in background without awaiting
        _ = WaitAndSendNotificationAsync(notification);
        return Task.CompletedTask;
    }

    private async Task SendNotificationAsync(INotification notification)
    {
        try
        {
            var eventMessage = new SignalREvent(
                notification.GetType().Name.Split('.').Last(),
                JsonConvert.SerializeObject(notification)
            );

            var serializedMessage = JsonConvert.SerializeObject(eventMessage);
            await _hubConnection.InvokeAsync("NewEventRaised", serializedMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish SignalR event.");
        }
    }

    private async Task WaitAndSendNotificationAsync(INotification notification)
    {
        try
        {
            const int maxRetries = 10;
            const int delayMs = 3000;

            int retryCount = 0;

            while (_hubConnection.ConnectionId == null && retryCount < maxRetries)
            {
                await Task.Delay(delayMs);
                retryCount++;
            }

            if (_hubConnection.ConnectionId != null)
            {
                await SendNotificationAsync(notification);
            }
            else
            {
                _logger.LogWarning("SignalR connection could not be established after retries. Notification dropped.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while waiting for SignalR connection.");
        }
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lifetime.ApplicationStarted.Register(() =>
        {
            _ = Task.Run(async () =>
            {
                await HandleSignalRMessages(cancellationToken);

                while (!_hubConnection.State.Equals(HubConnectionState.Connected) && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await _hubConnection.StartAsync(cancellationToken);
                        _logger.LogInformation("SignalR connection established. ConnectionId: {ConnectionId}", _hubConnection.ConnectionId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to connect to SignalR. Retrying in 5 seconds...");
                        await Task.Delay(5000, cancellationToken);
                    }
                }
            });
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
