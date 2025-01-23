using Microsoft.Extensions.Logging;

namespace LightMediator;
internal class Mediator : IMediator
{
    private readonly ILogger<Mediator> _logger;
    private readonly LightMediatorOptions _mediatorOptions;
    public IEnumerable<INotificationHandler> registeredServices { get; } = new List<INotificationHandler>();
    public Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger, LightMediatorOptions mediatorOptions)
    {
        registeredServices = serviceProvider.GetServices<INotificationHandler>();
        _logger = logger;
        _mediatorOptions = mediatorOptions;
    }
    public Task Publish(INotification notification, CancellationToken? cancellationToken = null)
    {
        var eventName = _mediatorOptions.IgnoreNamespaceInAssemblies
        ? notification.GetType().Name
        : notification.GetType().FullName;

        var matchingHandlers = registeredServices
            .Where(c => c.NotificationName.Equals(eventName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var handler in matchingHandlers)
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
        return Task.CompletedTask;
    }
}
