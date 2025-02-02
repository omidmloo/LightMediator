namespace ServiceC.BackService.Application.Commands;

public class TestCommandResponse : IRequest<bool>
{
    public string Title { get; set; }
    public string Description { get; set; }
}
