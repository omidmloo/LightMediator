namespace LightMediator.EventBus.MQTT.Exceptions;

public class MQTTConfigurationException : MediatorException
{
    public MQTTConfigurationException(string message) : base(message) { }

    public MQTTConfigurationException(string message, Exception innerException)
        : base(message, innerException) { }
}
