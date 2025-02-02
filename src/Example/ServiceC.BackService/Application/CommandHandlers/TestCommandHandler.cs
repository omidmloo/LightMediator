﻿using ServiceC.BackService.Application.Commands;

namespace ServiceC.BackService.Application.CommandHandlers;

public class TestCommandHandler : RequestHandler<TestCommand>
{
    public TestCommandHandler(LightMediatorOptions mediatorOptions) : base(mediatorOptions)
    {
    }
     
    public override async Task Handle(TestCommand request, CancellationToken? cancellationToken)
    {
        Console.WriteLine("command runned");
        await Task.CompletedTask;
    }
}
