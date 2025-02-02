
namespace LightMediator;

public abstract class NotificationHandler<TNotification> : INotificationHandler where TNotification : class, INotification
{
    public string NotificationName { get; }
    private readonly LightMediatorOptions _mediatorOptions;


    protected NotificationHandler(LightMediatorOptions mediatorOptions)
    {
        NotificationName = typeof(TNotification).Name;
        _mediatorOptions = mediatorOptions;
    }

    public Task HandleNotification(object message, CancellationToken? cancellationToken = null)
    {

        var json = JsonConvert.SerializeObject(message);
        var sourceFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        var targetFields = Activator.CreateInstance(typeof(TNotification));
        var targetProperties = typeof(TNotification).GetProperties();

        if (!_mediatorOptions.IgnoreNotificationDifferences &&
            (sourceFields == null ||
            sourceFields.Count() == 0))
        {
            throw new NullReferenceException($"Source notification has not property");
        }
        if (sourceFields == null)
            throw new ArgumentNullException();

        if (!_mediatorOptions.IgnoreNotificationDifferences && 
            sourceFields.Any(s => !targetProperties.Any(t => t.Name == s.Key)))
        {
            throw new InvalidCastException($"Cannot cast {NotificationName} - {typeof(TNotification).FullName} - Reciever notification has less fields");
        }
        foreach (var property in targetProperties)
        {
            if (sourceFields!.ContainsKey(property.Name) && property.CanWrite)
            {
                var value = sourceFields[property.Name];
                property.SetValue(targetFields, Convert.ChangeType(value, property.PropertyType));
            }
            else if (!_mediatorOptions.IgnoreNotificationDifferences)
            {
                throw new InvalidCastException($"Cannot cast {NotificationName} - Published notification has less fields");
            }
        }
        return Handle((TNotification)targetFields!, cancellationToken);
    }

    public abstract Task Handle(TNotification message, CancellationToken? cancellationToken);
}
