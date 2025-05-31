using WorkerService1;
using LightMediator;
using LightMediator.EventBus;
using LightMediator.EventBus.AzureServiceBus;
using LightMediator.EventBus.AzureServiceBus.Models; 
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
     .UseAzureServiceBus(builder.Configuration.GetSection("AzureServiceBusSettings"));
});
var host = builder.Build();

host.Run();
