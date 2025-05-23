var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();


// Add Mediator to the Dependency Injection container
builder.Services.AddLightMediator(options =>
{
    // Configure options for the mediator
    options.IgnoreNamespaceInAssemblies = true;
    options.IgnoreNotificationDifferences = true;
    options.RegisterNotificationsByAssembly = true;
    options.RegisterRequestsByAssembly = true;

    // specify the assemblies to scan for notifications
    options.Assemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        ServiceAExtensions.GetServiceAssembly(),
        ServiceBExtensions.GetServiceAssembly(),
        ServiceCExtensions.GetServiceAssembly()
    };
});

// Use hosting extension of services to configure 
builder.Services.AddServiceA();
builder.Services.AddServiceB();
builder.Services.AddServiceC();

var host = builder.Build();

host.Run();
