namespace LightMediator.EventBus;

public static class MediatorExtensions
{
    /// <summary>
    /// Publish an event to service bus
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="notification"></param>
    /// <param name="publishToEventBus"></param>
    /// <returns></returns>
    public static async Task PublishEvent(this IMediator mediator, INotification notification)
    {
        var eventName = notification.GetType().Name;
        var eventBuses = mediator.GetServiceProvider().GetServices<ILightMediatorEventBus>();
        var tasks = eventBuses.Select(eb => eb.PublishAsync(notification)).ToList();
        await Task.WhenAll(tasks);
    }

}
