namespace LightMediator.EventBus.AzureServiceBus.Exceptions;

public class AzureServiceBusConfigurationException : MediatorException
{
    public AzureServiceBusConfigurationException(string message) : base(message) { }

    public AzureServiceBusConfigurationException(string message, Exception innerException)
        : base(message, innerException) { }
}

