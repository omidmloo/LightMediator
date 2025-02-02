using ServiceC.BackService.Application.Commands;

namespace ServiceC.BackService.Application.CommandHandlers;

public class TestCommandResponseHandler : RequestHandler<TestCommandResponse,bool>
{
    private readonly ILogger<TestCommandResponseHandler> _logger;
    public TestCommandResponseHandler(LightMediatorOptions mediatorOptions, ILogger<TestCommandResponseHandler> logger) : base(mediatorOptions)
    {
        _logger = logger;
    }

    public override async Task<bool> Handle(TestCommandResponse request, CancellationToken? cancellationToken)
    {
        await Task.CompletedTask; 
        return false;
    }
}
