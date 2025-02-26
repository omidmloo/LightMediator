namespace LightMediator;

public interface IMediator
{
    /// <summary>
    /// Publishes a notification to all registered notification handlers.
    /// </summary>
    /// <param name="notification">The notification object to be published.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <param name="waitAllToPublish">
    /// If set to <c>true</c>, the method waits for all handlers to complete execution before returning.
    /// If <c>false</c>, handlers execute concurrently without awaiting completion.
    /// </param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task Publish(INotification notification, CancellationToken? cancellationToken = null, bool waitAllToPublish = false);
    /// <summary>
    /// Sends a request without expecting a response. This is typically used for commands or notifications.
    /// </summary>
    /// <typeparam name="TRequest">The type of request being sent.</typeparam>
    /// <param name="request">The request object to be processed.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task Send<TRequest>(TRequest request, CancellationToken? cancellationToken = null) where TRequest : class, IRequest;

    /// <summary>
    /// Sends a request and expects a response of type <typeparamref name="TResponse"/>.
    /// This is used for queries where a return value is required.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response expected.</typeparam>
    /// <param name="request">The request object that implements <see cref="IRequest{TResponse}"/>.</param>
    /// <param name="cancellationToken">An optional cancellation token.</param>
    /// <returns>A Task containing the response of type <typeparamref name="TResponse"/>.</returns>
    Task<TResponse?> Send<TResponse>(IRequest<TResponse> request, CancellationToken? cancellationToken = null);
}   
