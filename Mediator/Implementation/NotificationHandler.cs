
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
        var sourceFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
         
        var targetFields = Activator.CreateInstance(typeof(TNotification));
        var targetProperties = typeof(TNotification).GetProperties();

        foreach (var property in targetProperties)
        {
            if (sourceFields.ContainsKey(property.Name) && property.CanWrite)
            {
                var value = sourceFields[property.Name];
                property.SetValue(targetFields, Convert.ChangeType(value, property.PropertyType));
            }
        }

        return Handle((TNotification)targetFields, cancellationToken);
    }

    public virtual async Task Handle(TNotification message, CancellationToken? cancellationToken)
    {
        Console.WriteLine(message.ToString());
    }
}
