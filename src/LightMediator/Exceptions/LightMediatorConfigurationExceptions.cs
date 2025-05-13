using System.Reflection;

namespace LightMediator.Exceptions;

public class LightMediatorException : Exception
{
    public LightMediatorException(string message, Exception? inner = null)
        : base(message, inner) { }
}

public class LightMediatorOptionsException : LightMediatorException
{
    public LightMediatorOptionsException(string message)
        : base(message) { }
}

public class AssemblyScanningException : LightMediatorException
{
    public AssemblyScanningException(Assembly assembly, Exception? inner = null)
        : base($"Failed to scan assembly '{assembly.FullName}'.", inner) { }
}

public class InvalidHandlerRegistrationException : LightMediatorException
{
    public InvalidHandlerRegistrationException(Type handlerType, string reason)
        : base($"Handler type '{handlerType.FullName}' could not be registered. Reason: {reason}") { }
}

