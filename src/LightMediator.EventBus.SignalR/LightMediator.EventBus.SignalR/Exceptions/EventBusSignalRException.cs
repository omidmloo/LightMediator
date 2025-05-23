namespace LightMediator.EventBus.SignalR.Exceptions;

public class EventBusSignalRException : EventBusException
{
    public EventBusSignalRException(string message, Exception? inner = null)
        : base(message, inner) { }
}
