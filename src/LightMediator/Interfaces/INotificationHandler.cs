

namespace LightMediator;

public interface INotificationHandler
{
    string NotificationName { get; }
    internal Task HandleNotification(object message, CancellationToken? cancellationToken);
}
