namespace LightMediator.EventBus.SignalR.Exceptions;

public class SignalRConfigurationException : MediatorException
{
    public SignalRConfigurationException(string message) : base(message) { }

    public SignalRConfigurationException(string message, Exception innerException)
        : base(message, innerException) { }
}
