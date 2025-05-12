using Newtonsoft.Json;
using System.Threading;

namespace LightMediator.EventBus.SignalR;

internal class SignalREventBus : ILightMediatorEventBus, IHostedService
{
    private readonly IMediator _mediator;
    private readonly HubConnection _hubConnection;
    private readonly IHostApplicationLifetime _lifetime;

    public SignalREventBus(IMediator mediator, HubConnection hubConnection, IHostApplicationLifetime lifetime)
    {
        _mediator = mediator;
        _hubConnection = hubConnection;
        _lifetime = lifetime;
    }


    public Task OnEventRecieved(string notificationMessage, CancellationToken? cancellationToken = null)
    {
        var signalREvent = JsonConvert.DeserializeObject<SignalREvent>(notificationMessage);
        var type = Type.GetType(signalREvent.TypeName);
        if (type == null) return Task.CompletedTask;
        var notification = (INotification)JsonConvert.DeserializeObject(signalREvent.JsonPayload, type)!;
        return _mediator.Publish(notification, cancellationToken);
    }

    public async Task PublishAsync(INotification notification)
    {
        if (_hubConnection.ConnectionId == null) return;
        var eventMessage = new SignalREvent(notification.GetType().AssemblyQualifiedName!,
                                            JsonConvert.SerializeObject(notification));
        await _hubConnection.InvokeAsync("NewEventRaised", JsonConvert.SerializeObject(eventMessage));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lifetime.ApplicationStarted.Register(async () =>
        {
            await HandleSignalRMessages(cancellationToken);

            while (_hubConnection.ConnectionId == null)
            {
                await _hubConnection.StartAsync(cancellationToken);
                if (_hubConnection.ConnectionId == null)
                {
                    await Task.Delay(3000);
                    continue;
                }
            }
        });
        return Task.CompletedTask;
    }

    private Task HandleSignalRMessages(CancellationToken? cancellationToken = null)
    {
        _hubConnection.On<string>("EventRecieved", async message =>
        { 
            await OnEventRecieved(message, cancellationToken);
        });
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _hubConnection.StopAsync();
    }
}
