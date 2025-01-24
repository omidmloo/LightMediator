using System;

namespace ServiceB.BackService.Application.NotificationHandlers;

public class NewAppInfoNotificationHandler : NotificationHandler<NewAppInfoNotification>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NewAppInfoNotificationHandler> _logger;

    public NewAppInfoNotificationHandler(ILogger<NewAppInfoNotificationHandler> logger, IServiceProvider serviceProvider, LightMediatorOptions mediatorOptions) : base(mediatorOptions)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        //_mediator = mediator;
    }

    public override async Task Handle(NewAppInfoNotification notification, CancellationToken? cancellationToken)
    {
        // log gthat the event recieved
        _logger.LogInformation($"new notification recieved in service B {notification.GetType().Name}:{JsonConvert.SerializeObject(notification)}");
        // send new notification for serviceds C

        using (var scope = _serviceProvider.CreateScope())
        {
        NewProtectionTemplateNotification newProtectionTemplate = new NewProtectionTemplateNotification()
        {
            AppName = notification.Title
        };

            var mediator = scope.ServiceProvider.GetService<IMediator>()!;
            await mediator.Publish(newProtectionTemplate);
        } 
    }
}
