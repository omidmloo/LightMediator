using LightMediator;
using LightMediator.EventBus;
using WorkerService1.Events;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMediator _mediator;

        public Worker(ILogger<Worker> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
            int i = 1;
            while (!stoppingToken.IsCancellationRequested)
            {
                await _mediator.PublishEvent(new TestEvent() { MyProperty = $"Hello {i}" });
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(5000, stoppingToken);
                i++;
            }
        }
    }
}
