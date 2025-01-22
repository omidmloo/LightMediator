namespace AppMediator;
internal class Mediator : IMediator
{
    public IEnumerable<INotificationHandler> registeredServices { get; } = new List<INotificationHandler>();
    public Mediator(IServiceProvider serviceProvider)
    {
        registeredServices = serviceProvider.GetServices<INotificationHandler>();
    }
    public async Task Publish(INotification notification, CancellationToken? cancellationToken = null)
    {
        var eventName = notification.GetType().Name;
        /// Get all event handlers registered by the event name of notification
        /// Raise handle for all handlers
        if (registeredServices.Any(c => c.NotificationName.Equals(eventName, StringComparison.OrdinalIgnoreCase)))
        {
            var handlers = registeredServices.Where(c => c.NotificationName.Equals(eventName, StringComparison.OrdinalIgnoreCase))
                            .ToList().Select(c =>
                            {
                                return c.HandleNotification(notification, cancellationToken);
                            });
            await Task.WhenAll(handlers); 
        }
    }
}
