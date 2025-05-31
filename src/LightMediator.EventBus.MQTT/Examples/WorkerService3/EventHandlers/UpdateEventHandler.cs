using LightMediator;
using WorkerService3.Eventss;

namespace WorkerService2.EventHandlers;

internal class UpdateEventHandler : NotificationHandler<UpdateEvent>
{
    private readonly ILogger<UpdateEventHandler> _logger; 

    public UpdateEventHandler(ILogger<UpdateEventHandler> logger)
    {
        _logger = logger; 
    }

    public override async Task Handle(UpdateEvent message, CancellationToken? cancellationToken)
    {
        _logger.LogInformation($"___________________Event recieved__________________ {message.Action}");
    }
}
