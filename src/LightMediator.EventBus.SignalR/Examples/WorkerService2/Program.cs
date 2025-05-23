using WorkerService2;
using LightMediator;
using LightMediator.EventBus;
using LightMediator.EventBus.SignalR;
using LightMediator.EventBus.SignalR.Models;
using System.Reflection;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddLightMediator(o =>
{
    o.IgnoreNamespaceInAssemblies = true;
    o.IgnoreNotificationDifferences = true;
    o.RegisterNotificationsByAssembly = true;
    o.RegisterRequestsByAssembly = true;
    o.Assemblies = new[]
            {
                Assembly.GetExecutingAssembly()
            };
    o.AddLightMediatorEventBus(builder.Services)
     .UseSignalRService(new SignalRSettings()
     {
         ServerAddress = "https://localhost:7223",
     });
});
var host = builder.Build();
host.Run();
