using Microsoft.Extensions.Hosting;
using WorkerService3.Token;

namespace WorkerService3
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITokenHanlder _tokenHanlder;

        public Worker(ILogger<Worker> logger, ITokenHanlder tokenHanlder)
        {
            _logger = logger;
            _tokenHanlder = tokenHanlder;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                  //  _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                //if (i == 10)
                //    await _tokenHanlder.TokenChanged();
                await Task.Delay(1000, stoppingToken);
                i++;
            }
        }
    }
}
