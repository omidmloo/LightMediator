namespace FinalService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMediator _mediator;
        public Worker(ILogger<Worker> logger,IMediator mediator)
        {
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _mediator.Send(new TestCommand()
                {
                    Title = "Test",
                    Description = "Test",
                });

                var res = await _mediator.Send<bool>(new TestCommandResponse()
                {
                    Title = "Test",
                    Description = "Test",
                });

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
