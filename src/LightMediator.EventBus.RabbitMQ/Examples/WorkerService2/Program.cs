using WorkerService2;
using LightMediator;
using LightMediator.EventBus;
using LightMediator.EventBus.RabbitMQ;
using LightMediator.EventBus.RabbitMQ.Models;
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
     .UseRabbitMQ(new RabbitMQSettings()
     {
         HostUri = "rabbitmq://localhost",
         Username = "localuser",
         Password = "localpassword"
     });
});
var host = builder.Build();
host.Run();
