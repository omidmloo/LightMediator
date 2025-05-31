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
            await Task.Delay(2000, stoppingToken);
            int i = 1;
            while (!stoppingToken.IsCancellationRequested)
            {
                var t1 = _mediator.PublishEvent(new TestEvent() { MyProperty = $"Hello pro {i}" }, extraTopics: "pro");
                var t2 = _mediator.PublishEvent(new TestEvent() { MyProperty = $"Hello plus {i}" }, extraTopics: "plus");
                await Task.WhenAll(t1, t2);
                if (_logger.IsEnabled(LogLevel.Information))
                {
                   // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(2000, stoppingToken);
                i++;
            }
        }
    }
}
