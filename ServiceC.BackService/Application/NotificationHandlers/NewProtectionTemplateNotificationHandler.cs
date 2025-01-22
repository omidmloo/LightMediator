namespace ServiceC.BackService.Application.NotificationHandlers;

public class NewProtectionTemplateNotificationHandler : NotificationHandler<NewProtectionTemplateNotification>
{
    private readonly ILogger<NewProtectionTemplateNotificationHandler> _logger; 

    public NewProtectionTemplateNotificationHandler(ILogger<NewProtectionTemplateNotificationHandler> logger)  
    {
        _logger = logger; 
    }
     
    public override async Task Handle(NewProtectionTemplateNotification notification, CancellationToken? cancellationToken)
    {
        // log gthat the event recieved
        _logger.LogInformation($"new notification recieved in service C {notification.GetType().Name}: {JsonConvert.SerializeObject(notification)}");
         
    }
}