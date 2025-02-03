

namespace LightMediator;

public interface INotificationHandler
{
    string NotificationName { get; }
    internal Task HandleNotification(object message,LightMediatorOptions mediatorOptions, CancellationToken? cancellationToken);
}
