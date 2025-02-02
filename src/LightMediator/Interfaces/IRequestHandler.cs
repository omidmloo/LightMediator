

namespace LightMediator;

public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    string RequestName { get; }
    Task<TResponse> HandleRequest(object request, CancellationToken? cancellationToken);
}

public interface IRequestHandler<TRequest> where TRequest : IRequest 
{
    string RequestName { get; }
    Task HandleRequest(object request, CancellationToken? cancellationToken);
}
