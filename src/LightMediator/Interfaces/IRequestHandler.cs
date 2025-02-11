

namespace LightMediator;

internal interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    string RequestName { get; }
    Task<TResponse> HandleRequest(object request, LightMediatorOptions mediatorOptions, CancellationToken? cancellationToken);
}

internal interface IRequestHandler<TRequest> where TRequest : IRequest 
{
    string RequestName { get; }
    Task HandleRequest(object request, LightMediatorOptions mediatorOptions, CancellationToken? cancellationToken);
}
