
# LightMediator

**LightMediator** is a lightweight library designed to simplify decoupled communication in distributed Windows services. It works with multiple notification types and supports services across different namespaces.

## Features
- Lightweight and efficient.
- Supports publish-subscribe, request-response, and one-way notifications.
- Simplifies working with decoupled services in distributed systems.
- Easy to integrate with existing applications.

## Installation
You can install the LightMediator NuGet package using the following command:
```bash
dotnet add package LightMediator
```

## Usage

### Publish Notifications
You can publish notifications to subscribers using the LightMediator instance:
```csharp
var mediator = new LightMediator();
mediator.Publish(new Notification("ServiceStarted"));
```

### Handle Notifications
To handle notifications, create a class that implements the `INotificationHandler<T>` interface. Ensure that `Notification` implements the `INotification` interface:
```csharp
public class Notification : INotification
{
    public string Message { get; }

    public Notification(string message)
    {
        Message = message;
    }
}

public class NotificationHandler : INotificationHandler<Notification>
{
    public Task Handle(Notification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received: {notification.Message}");
        return Task.CompletedTask;
    }
}
```

## License
This project is licensed under the MIT License. See the LICENSE file for details.
