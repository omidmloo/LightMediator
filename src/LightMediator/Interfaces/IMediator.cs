namespace LightMediator;

public interface IMediator
{
    Task Publish(INotification notification, CancellationToken? cancellationToken = null);
    Task Send<TRequest>(TRequest request, CancellationToken? cancellationToken = null) where TRequest : class, IRequest;
    Task<TResponse?> Send<TResponse>(IRequest<TResponse> request, CancellationToken? cancellationToken = null);
}   
