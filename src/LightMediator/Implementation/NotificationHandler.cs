namespace LightMediator; 

public abstract class NotificationHandler<TNotification> : INotificationHandler
    where TNotification : class, INotification
{
    public string NotificationName { get; }

    private readonly ILogger? _logger;

    protected NotificationHandler(ILogger<NotificationHandler<TNotification>>? logger = null)
    {
        NotificationName = typeof(TNotification).Name;
        _logger = logger;
    }

    public async Task HandleNotification(object message, LightMediatorOptions mediatorOptions, CancellationToken? cancellationToken = null)
    {
        try
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Notification message is null.");

            var json = JsonConvert.SerializeObject(message);
            var notification = JsonConvert.DeserializeObject<TNotification>(json);

            if (notification == null)
                throw new NotificationDeserializationException(typeof(TNotification), json);

            await Handle(notification, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling notification of type {NotificationType}.", typeof(TNotification).Name);
            throw new NotificationHandlingException(typeof(TNotification), ex);
        }
    }

    public abstract Task Handle(TNotification message, CancellationToken? cancellationToken);
}

