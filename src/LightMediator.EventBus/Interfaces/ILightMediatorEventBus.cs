namespace LightMediator.EventBus;

public interface ILightMediatorEventBus
{
    Task PublishAsync(INotification notification);
    Task OnEventRecieved(string notificationMessage, CancellationToken? cancellationToken);
}
