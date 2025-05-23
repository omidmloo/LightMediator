namespace LightMediator.EventBus.Exceptions;

public class EventBusException : Exception
{
    public EventBusException(string message, Exception? inner = null)
        : base(message, inner) { }
}

public class EventBusPublishException : MediatorException
{
    public EventBusPublishException(Type eventBusType, Type notificationType, Exception inner)
        : base($"Failed to publish '{notificationType.FullName}' on event bus '{eventBusType.FullName}'.", inner) { }
}

public class EventBusAggregateException : MediatorException
{
    public EventBusAggregateException(Type notificationType, IEnumerable<Exception> innerExceptions)
        : base($"One or more errors occurred while publishing notification '{notificationType.FullName}'.", new AggregateException(innerExceptions)) { }
}