# LightMediator

[![NuGet](https://img.shields.io/nuget/v/LightMediator.svg)](https://www.nuget.org/packages/LightMediator)
[![License](https://img.shields.io/github/license/omidmloo/LightMediator)](LICENSE)

**LightMediator** is a lightweight library designed to simplify decoupled communication in distributed Windows services. It works with multiple notification types and supports services across different namespaces.

## Features
- Lightweight and efficient.
- Supports publish-subscribe, request-response, and one-way notifications.
- Simplifies working with decoupled services in distributed systems.
- Easy to integrate with existing applications.
- Supports commands and command handlers for request-response communication.

## Installation
You can install the LightMediator NuGet package using the following command:
```bash
dotnet add package LightMediator
```

## Usage

### Publish Notifications
You can publish notifications to subscribers using the LightMediator instance:
```csharp
  private readonly IMediator _mediator;
  public SampleService(IMediator mediator)
  {
      _deviceService = deviceService;
  }
.
.
    NewAppInfoNotification newAppInfo = new NewAppInfoNotification()
    {
        Title = "NewApp",
        Description = "Des"
    };
    await mediator.Publish(newAppInfo);
```

### Handle Notifications
To handle notifications, create a class that implements the `INotificationHandler<T>` interface. Ensure that `Notification` implements the `INotification` interface:
```csharp
public class NewAppInfoNotification : INotification
{
    public string Title { get; set; }
    public string? Description { get; set; }
}


public class NewAppInfoNotificationHandler : NotificationHandler<NewAppInfoNotification>
{ 
    public override async Task Handle(NewAppInfoNotification notification, CancellationToken? cancellationToken)
    {
           // Handle the recieved notification
    }
}

```

### Use Commands and Command Handlers
LightMediator also supports **commands** and **command handlers** to facilitate request-response communication. 

#### Define a Command
A command is a class that implements the `IRequest<TResponse>` interface, where `TResponse` is the type of the response the command expects.

```csharp

public class TestCommandWithResponse : IRequest<TestCommandResponse>
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public class TestCommandResponse
{
    public string Message { get; set; }
}
```

#### Define a Command Handler
A command handler is a class that implements the `IRequestHandler<TRequest, TResponse>` interface. The handler contains the logic for processing the command.

```csharp

public class TestCommandWithResponseHandler : RequestHandler<TestCommandWithResponse, TestCommandResponse>
{
    private readonly ILogger<TestCommandWithResponseHandler> _logger;
    public TestCommandWithResponseHandler(ILogger<TestCommandWithResponseHandler> logger) 
    {
        _logger = logger;
    }

    public override async Task<TestCommandResponse> Handle(TestCommandWithResponse request, CancellationToken? cancellationToken)
    {
        await Task.CompletedTask; 
        return new TestCommandResponse
        {
            Message = "command executed"
        };
    }
}


``` 

#### Sending a Command
You can send a command and get a response using the `Send` method from LightMediator.

```csharp
 public class Worker : BackgroundService
 {
     private readonly ILogger<Worker> _logger;
     private readonly IMediator _mediator;
     public Worker(ILogger<Worker> logger,IMediator mediator)
     {
         _mediator = mediator;
         _logger = logger;
     }

     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
     {
         while (!stoppingToken.IsCancellationRequested)
         {
             var res = await _mediator.Send<bool>(new TestCommandResponse()
             {
                 Title = "Test",
                 Description = "Test",
             });

             if (_logger.IsEnabled(LogLevel.Information))
             {
                 _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
             }
             await Task.Delay(1000, stoppingToken);
         }
     }
 }
```

### Registering Commands , Notifications and Handlers
You can register handlers with the dependency injection container in your application startup:

```csharp
// Add Mediator to the Dependency Injection container
builder.Services.AddLightMediator(options =>
{
    // Configure options for the mediator
    options.IgnoreNamespaceInAssemblies = true;
    options.IgnoreNotificationDifferences = true;
    options.RegisterNotificationsByAssembly = true;
    options.RegisterRequestsByAssembly = true;

    //Specify the assemblies to scan for notification and request
    options.Assemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        ServiceAExtensions.GetServiceAssembly(),
        ServiceBExtensions.GetServiceAssembly(),
        ServiceCExtensions.GetServiceAssembly()
    };
});
```

## License
This project is licensed under the MIT License. See the LICENSE file for details. 

---

> Created with ❤️ by [@omidmloo](https://github.com/omidmloo) 
 
