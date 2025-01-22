using Microsoft.Extensions.Logging;

namespace AppMediator;
internal class Mediator : IMediator
{
    private readonly ILogger<Mediator> _logger;
    public IEnumerable<INotificationHandler> registeredServices { get; } = new List<INotificationHandler>();
    public Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger)
    {
        registeredServices = serviceProvider.GetServices<INotificationHandler>();
        _logger = logger;
    }
    public async Task Publish(INotification notification, CancellationToken? cancellationToken = null)
    {
        var eventName = notification.GetType().Name;
        /// Get all event handlers registered by the event name of notification
        /// Raise handle for all handlers
        if (registeredServices.Any(c => c.NotificationName.Equals(eventName, StringComparison.OrdinalIgnoreCase)))
        {
            var handlers = registeredServices.Where(c => c.NotificationName.Equals(eventName, StringComparison.OrdinalIgnoreCase))
                            .ToList();
            foreach (var handler in handlers)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await handler.HandleNotification(notification, cancellationToken);
                    }
                    catch (Exception ex)
                    { 
                        _logger.LogError($"Error in handler: {ex.Message}");
                    }
                });
                 
            }
        }
    }
}
