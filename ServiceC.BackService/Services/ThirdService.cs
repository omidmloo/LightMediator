 
namespace ServiceC.BackService.Services;

public class ThirdService : BackgroundService
{ 
    private readonly ILogger<ThirdService> _logger;

    public ThirdService(ILogger<ThirdService> logger)
    { 
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation($"Service C is up and running");

            while (!stoppingToken.IsCancellationRequested)
            { 
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            Environment.Exit(1);
        }
    }
}
