 
# LightMediator.EventBus

[![NuGet](https://img.shields.io/nuget/v/LightMediator.EventBus.svg)](https://www.nuget.org/packages/LightMediator.EventBus)
[![License](https://img.shields.io/github/license/omidmloo/LightMediator.EventBus)](LICENSE)

LightMediator.EventBus is an extension of the [LightMediator](https://github.com/omidmloo/LightMediator) library that provides support for EventBus-style communication between services, applications, or distributed components in a decoupled and efficient way.

---

## ✨ Features

- 🔁 **Publish-Subscribe pattern** built on top of LightMediator
- 🧩 **Extensible architecture** – plug into different transport layers (e.g., RabbitMQ, Azure Service Bus, SignalR, etc.)
- 🛠️ **Seamless integration** with existing LightMediator pipelines
- 📦 **Lightweight and performant** – zero external dependencies unless needed by transport
- ✅ **Unit-test friendly** design

---

## 📦 Installation

Install via NuGet:

```bash
dotnet add package LightMediator.EventBus
````

---

## 🚀 Getting Started

```csharp
public class OrderCreatedEvent : IEvent
{
    public Guid OrderId { get; init; }
}

public class OrderCreatedHandler : IEventHandler<OrderCreatedEvent>
{
    public Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Order created: {@event.OrderId}");
        return Task.CompletedTask;
    }
}
```

Then register the EventBus in your DI container:

```csharp 

services.AddLightMediator(options =>   // from LightMediator
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
        Lib1.GetServiceAssembly(),
        Lib2.GetServiceAssembly(), 
        Service1.GetServiceAssembly()
    };
    options.AddLightMediatorEventBus(services);   // from this package
},
```

Publish an event:
Use PublishEvent extension method for light mediator to publish cross domain events
```csharp
await lightMediator.PublishEvent(new OrderCreatedEvent { OrderId = Guid.NewGuid() });
```

---

## 🧱 Architecture

This library follows a **simple EventBus pattern** and leverages the core pipeline and handler resolution features of [LightMediator](https://github.com/omidmloo/LightMediator). It abstracts away transport-specific logic so developers can focus on events and handlers.

---

## 📄 Dependencies

This package depends on:

* [`LightMediator`](https://github.com/omidmloo/LightMediator): A minimalistic and performant in-process mediator pattern library for .NET.


## ✅ Contributing

Contributions are welcome! Please feel free to open issues, submit pull requests, or suggest enhancements.

---

## 📝 License

This project is licensed under the [MIT License](LICENSE).

---

## 💬 Contact

For support or questions, feel free to reach out via GitHub Issues or contact [@omidmloo](https://github.com/omidmloo).
