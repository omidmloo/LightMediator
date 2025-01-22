
namespace AppMediator;

public abstract class NotificationHandler<TNotification> : INotificationHandler where TNotification : class, INotification
{
    public string NotificationName { get; }

    protected NotificationHandler()
    {
        NotificationName = typeof(TNotification).Name; 
    }

    public Task HandleNotification(object message, CancellationToken? cancellationToken = null)
    {
        var json = JsonConvert.SerializeObject(message);
        return Handle(JsonConvert.DeserializeObject<TNotification>(json), cancellationToken);
    }

    public virtual async Task Handle(TNotification message, CancellationToken? cancellationToken)
    {
        Console.WriteLine(message.ToString());
    }
}
