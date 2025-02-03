using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceA.BackService.Application.Notifications;

namespace ServiceA.BackService.Application.NotificationHandlers;

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
        _logger.LogInformation($"new notification recieved in service A {notification.GetType().Name}: {JsonConvert.SerializeObject(notification)}");
        
    }
}
