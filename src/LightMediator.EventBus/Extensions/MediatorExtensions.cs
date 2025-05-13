namespace LightMediator.EventBus;

public static class MediatorExtensions
{
    /// <summary>
    /// Publish an event to all registered event buses.
    /// </summary>
    /// <param name="mediator">Mediator instance</param>
    /// <param name="notification">Notification to publish</param>
    /// <returns></returns>
    public static async Task PublishEvent(this IMediator mediator, INotification notification)
    {
        if (mediator == null) throw new ArgumentNullException(nameof(mediator));
        if (notification == null) throw new ArgumentNullException(nameof(notification));

        var eventBuses = mediator.GetServiceProvider().GetServices<ILightMediatorEventBus>().ToList();

        var exceptions = new List<Exception>();
        var tasks = new List<Task>();

        foreach (var eventBus in eventBuses)
        {
            tasks.Add(SafePublish(eventBus, notification, exceptions));
        }

        await Task.WhenAll(tasks);

        if (exceptions.Any())
        {
            throw new EventBusAggregateException(notification.GetType(), exceptions);
        }
    }

    private static async Task SafePublish(ILightMediatorEventBus eventBus, INotification notification, List<Exception> exceptions)
    {
        try
        {
            await eventBus.PublishAsync(notification);
        }
        catch (Exception ex)
        {
            exceptions.Add(new EventBusPublishException(eventBus.GetType(), notification.GetType(), ex));
        }
    }

}
