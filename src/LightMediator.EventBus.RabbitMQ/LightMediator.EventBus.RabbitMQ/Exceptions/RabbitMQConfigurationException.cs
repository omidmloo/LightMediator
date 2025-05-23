namespace LightMediator.EventBus.RabbitMQ.Exceptions;

public class RabbitMQConfigurationException : MediatorException
{
    public RabbitMQConfigurationException(string message) : base(message) { }

    public RabbitMQConfigurationException(string message, Exception innerException)
        : base(message, innerException) { }
}

