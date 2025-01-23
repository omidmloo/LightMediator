var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();


// Add Mediator to the Dependency Injection container
builder.Services.AddLightMediator(options =>
{
    // Configure options for the mediator
    options.IgnoreNamespaceInAssemblies = true;
    options.RegisterNotificationsByAssembly = true; 

    // specify the assemblies to scan for notifications
    options.Assemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        ServiceA.BackService.Extensions.ServiceExtensions.GetServiceAssembly(),
        ServiceB.BackService.Extensions.ServiceExtensions.GetServiceAssembly(),
        ServiceC.BackService.Extensions.ServiceExtensions.GetServiceAssembly()
    };
});

// Use hosting extension of services to configure 
builder.Services.AddServiceA();
builder.Services.AddServiceB();
builder.Services.AddServiceC();

var host = builder.Build();

host.Run();
