namespace LightMediator.Exceptions;

public class NotificationHandlingException : MediatorException
{
    public NotificationHandlingException(Type notificationType, Exception innerException)
        : base($"An error occurred while handling notification of type '{notificationType.FullName}'.", innerException) { }
}

public class NotificationDeserializationException : MediatorException
{
    public NotificationDeserializationException(Type targetType, string json)
        : base($"Failed to deserialize notification to type '{targetType.FullName}'. JSON: {json}") { }
}
