using LightMediator;
using WorkerService3.Events;

namespace WorkerService3.EventHandlers;

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
