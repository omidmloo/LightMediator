

namespace LightMediator;

internal interface INotificationHandler
{
    string NotificationName { get; }
    Task HandleNotification(object message,LightMediatorOptions mediatorOptions, CancellationToken? cancellationToken);
}
