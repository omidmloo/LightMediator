using ServiceC.BackService.Application.Commands;

namespace ServiceC.BackService.Application.CommandHandlers;

public class TestCommandWithResponseHandler : RequestHandler<TestCommandWithResponse, TestCommandResponse>
{
    private readonly ILogger<TestCommandWithResponseHandler> _logger;
    public TestCommandWithResponseHandler(ILogger<TestCommandWithResponseHandler> logger) 
    {
        _logger = logger;
    }

    public override async Task<TestCommandResponse> Handle(TestCommandWithResponse request, CancellationToken? cancellationToken)
    {
        await Task.CompletedTask; 
        return new TestCommandResponse
        {
            Message = "command executed"
        };
    }
}
