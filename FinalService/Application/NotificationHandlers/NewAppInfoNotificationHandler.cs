using Newtonsoft.Json;

namespace FinalService.Application.NotificationHandlers;

public class NewAppInfoNotificationHandler : NotificationHandler<NewAppInfoNotification>
{
    private readonly ILogger<NewAppInfoNotificationHandler> _logger;

    public NewAppInfoNotificationHandler(ILogger<NewAppInfoNotificationHandler> logger)
    {
        _logger = logger;
    }

    public override async Task Handle(NewAppInfoNotification notification, CancellationToken? cancellationToken)
    {
        // log gthat the event recieved
        _logger.LogInformation($"new notification recieved in FINAL SERVICE {notification.GetType().Name}: {JsonConvert.SerializeObject(notification)}");

        await Task.CompletedTask;

    }
}
