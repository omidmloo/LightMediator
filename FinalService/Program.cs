using ServiceC.BackService.Application;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddAppMediator();
builder.Services.RegisterNotificationsFromAssemblies(
                                Assembly.GetExecutingAssembly(),
                                 ServiceA.BackService.Extensions.ServiceExtensions.GetServiceAssembly(),
                                 ServiceB.BackService.Extensions.ServiceExtensions.GetServiceAssembly(),
                                 ServiceC.BackService.Extensions.ServiceExtensions.GetServiceAssembly()
                            );

builder.Services.AddServiceA();
builder.Services.AddServiceB();
builder.Services.AddServiceC();

var host = builder.Build();

host.Run();
