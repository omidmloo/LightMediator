# LightMediator.EventBus.RabbitMQ

[![NuGet](https://img.shields.io/nuget/v/LightMediator.EventBus.RabbitMQ.svg)](https://www.nuget.org/packages/LightMediator.EventBus.RabbitMQ/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**LightMediator.EventBus.RabbitMQ** is an extension for [LightMediator](https://github.com/omidmloo/LightMediator) and [LightMediator.EventBus](https://github.com/omidmloo/LightMediator.EventBus) that adds distributed messaging support using RabbitMQ.

It enables publishing and subscribing to `INotification` messages between services, applications, or domains in a decoupled and scalable way — powered by RabbitMQ and your high-performance, lightweight mediator.

## ✨ Features

- 🔄 Publish/Subscribe model over RabbitMQ.
- 💡 Built on top of `LightMediator.EventBus`.
- 🔌 Easy integration with minimal configuration.
- 🔒 Supports durable queues and automatic reconnects. 

## 📦 Installation

Install via NuGet:

```bash
dotnet add package LightMediator.EventBus.RabbitMQ
````

Or via the Package Manager:

```powershell
Install-Package LightMediator.EventBus.RabbitMQ
```

## 🚀 Getting Started

### 1. Register LightMediator with RabbitMQ support

In your `Program.cs` or `Startup.cs`:

```csharp
builder.Services.AddLightMediator(options =>
{
    options.IgnoreNamespaceInAssemblies = true;
    options.IgnoreNotificationDifferences = true;
    options.RegisterNotificationsByAssembly = true;
    options.RegisterRequestsByAssembly = true;

    options.Assemblies = new[]
    {
        Assembly.GetExecutingAssembly(),
        Lib1.GetServiceAssembly(),
        Lib2.GetServiceAssembly(),
        Service1.GetServiceAssembly()
    };

    options.AddLightMediatorEventBus(services)
           .UseRabbitMQ(new RabbitMQSettings()
             {
                 HostUri = "rabbitmq://localhost",
                 Username = "guest",
                 Password = "guestpassword"
             };
});
```

### 2. Create a Notification

```csharp
using LightMediator;

namespace WorkerService1.Events;

internal class TestEvent:INotification
{
    public string MyProperty { get; set; }
}
```

### 3. Handle the Notification in another service

```csharp
using LightMediator;
using WorkerService2.Events;

namespace WorkerService2.EventHandlers;

internal class TestEventHandler : NotificationHandler<TestEvent>
{
    private readonly ILogger<TestEventHandler> _logger;

    public TestEventHandler(ILogger<TestEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task Handle(TestEvent message, CancellationToken? cancellationToken)
    {
        _logger.LogInformation($"___________________Event recieved__________________ {message.MyProperty}");
        return Task.CompletedTask;
    }
}

```

### 4. Publish a Notification as a Event using eventbus extension method

```csharp
 await _mediator.PublishEvent(new TestEvent() { MyProperty = "Hello"});
```

> The message will be delivered to all services that registered the same `ExchangeName` and are listening for that notification.


## 📝 License

This project is licensed under the [MIT License](LICENSE).

---

> Created with ❤️ by [@omidmloo](https://github.com/omidmloo)