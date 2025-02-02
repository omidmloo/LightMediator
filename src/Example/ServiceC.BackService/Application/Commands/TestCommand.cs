namespace ServiceC.BackService.Application.Commands;

public class TestCommand : IRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
}
