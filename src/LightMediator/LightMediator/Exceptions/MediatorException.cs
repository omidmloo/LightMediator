namespace LightMediator.Exceptions;


public class MediatorException : Exception
{
    public MediatorException(string message, Exception? inner = null)
        : base(message, inner) { }
}

public class HandlerNotFoundException : MediatorException
{
    public HandlerNotFoundException(string requestName)
        : base($"No handler found for request: {requestName}") { }
}

public class RequestInvocationException : MediatorException
{
    public RequestInvocationException(Type handlerType, Exception? inner = null)
        : base($"Failed to invoke request handler '{handlerType.FullName}'.", inner) { }
}

public class InvalidHandlerInterfaceException : MediatorException
{
    public InvalidHandlerInterfaceException(Type handlerType)
        : base($"Handler type '{handlerType.FullName}' does not implement a valid request handler interface.") { }
}
