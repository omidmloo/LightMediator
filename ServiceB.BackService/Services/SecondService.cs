namespace ServiceB.BackService.Services;

public class SecondService : BackgroundService
{ 
    private readonly ILogger<SecondService> _logger;

    public SecondService(ILogger<SecondService> logger)
    { 
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation($"Service B is up and running");

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
