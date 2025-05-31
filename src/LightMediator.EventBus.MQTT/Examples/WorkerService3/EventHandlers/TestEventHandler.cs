using LightMediator;
using WorkerService3.Events;
using WorkerService3.Token;

namespace WorkerService2.EventHandlers;

internal class TestEventHandler : NotificationHandler<TestEvent>
{
    private readonly ILogger<TestEventHandler> _logger;
    private readonly ITokenHanlder _tokenHanlder;

    public TestEventHandler(ILogger<TestEventHandler> logger, ITokenHanlder tokenHanlder)
    {
        _logger = logger;
        _tokenHanlder = tokenHanlder;
    }

    public override async Task Handle(TestEvent message, CancellationToken? cancellationToken)
    {
        _logger.LogInformation($"___________________Event recieved__________________ {message.MyProperty}");
        await Task.Delay(3000);
        //await _tokenHanlder.TokenChanged +=> (){

        //};
    }
}
