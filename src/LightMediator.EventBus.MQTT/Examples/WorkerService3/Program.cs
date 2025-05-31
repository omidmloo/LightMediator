using WorkerService3;
using LightMediator;
using LightMediator.EventBus; 
using System.Reflection;
using WorkerService3.Token;
using LightMediator.EventBus.MQTT;
using LightMediator.EventBus.RabbitMQ.Models;
using LightMediator.EventBus.RabbitMQ;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<ITokenHanlder, TokenHandler>();
builder.Services.AddSingleton<IMQTTTokenHandler, MQTTTokenHandler>();
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
     .UseMQTT(
           builder.Configuration.GetSection("MQTTSettings"), mqtto =>
                                    mqtto.SetTokenHandler<TargetException>().AddTopics("pro"));
}); 

var host = builder.Build();
host.Run();
