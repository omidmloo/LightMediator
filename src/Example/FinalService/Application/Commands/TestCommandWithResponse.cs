namespace FinalService.Application.Commands;

public class TestCommandWithResponse : IRequest<TestCommandResponse>
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public class TestCommandResponse
{
    public string Message { get; set; }
}