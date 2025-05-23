namespace LightMediator.EventBus.SignalR.Exceptions;

public class SignalRConnectionException : EventBusSignalRException
{
    public SignalRConnectionException(string message, Exception? inner = null)
        : base(message, inner) { }
}

