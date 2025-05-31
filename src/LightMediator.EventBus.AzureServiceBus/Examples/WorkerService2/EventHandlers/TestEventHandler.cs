using LightMediator;
using WorkerService2.Events;

namespace WorkerService2.EventHandlers;

internal class TestEventHandler : NotificationHandler<TestEvent>
{
    private readonly ILogger<TestEventHandler> _logger;

    public TestEventHandler(ILogger<TestEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(TestEvent message, CancellationToken? cancellationToken)
    {
        _logger.LogInformation($"___________________Event recieved__________________ {message.MyProperty}");
        return Task.CompletedTask;
    }
}
