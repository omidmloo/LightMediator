
namespace LightMediator;

public abstract class NotificationHandler<TNotification> : INotificationHandler where TNotification : class, INotification
{
    public string NotificationName { get; } 


    protected NotificationHandler()
    {
        NotificationName = typeof(TNotification).Name; 
    }

    public Task HandleNotification(object message, LightMediatorOptions mediatorOptions, CancellationToken? cancellationToken = null)
    {

        var source = JsonConvert.SerializeObject(message); 
        var target = JsonConvert.DeserializeObject<TNotification>(source)!; 
        return Handle(target, cancellationToken);
    }

    public abstract Task Handle(TNotification message, CancellationToken? cancellationToken);
}
