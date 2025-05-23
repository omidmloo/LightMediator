namespace LightMediator.EventBus.Exceptions;

public class EventDeserializationException : EventBusException
{
    public EventDeserializationException(string message, Exception? inner = null)
        : base(message, inner) { }
}

