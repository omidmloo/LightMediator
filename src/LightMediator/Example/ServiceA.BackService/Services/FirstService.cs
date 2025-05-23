using Microsoft.Extensions.Logging;
using ServiceA.BackService.Application.Notifications;

namespace ServiceA.BackService.Services;

public class FirstService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FirstService> _logger;

    public FirstService(ILogger<FirstService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation($"Service A is up and running");
            while (!stoppingToken.IsCancellationRequested)
            {
                NewAppInfoNotification newAppInfo = new NewAppInfoNotification()
                {
                    Title = "NewApp",
                    Description = "Des"
                };
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetService<IMediator>()!;   
                    await mediator.Publish(newAppInfo, stoppingToken, true);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            Environment.Exit(1);
        }
    }
}
