namespace LightMediator.Exceptions;

public class RequestHandlingException : MediatorException
{
    public RequestHandlingException(Type requestType, Exception innerException)
        : base($"An error occurred while handling request of type '{requestType.FullName}'.", innerException) { }
}


public class RequestDeserializationException : MediatorException
{
    public RequestDeserializationException(Type requestType, string json)
        : base($"Failed to deserialize request to type '{requestType.FullName}'. JSON: {json}") { }
}
