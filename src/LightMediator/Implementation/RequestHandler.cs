
namespace LightMediator;
public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : class, IRequest
{
    public string RequestName { get; }

    private readonly ILogger? _logger;

    protected RequestHandler(ILogger<RequestHandler<TRequest>>? logger = null)
    {
        RequestName = typeof(TRequest).Name;
        _logger = logger;
    }

    public async Task HandleRequest(object request, LightMediatorOptions mediatorOptions, CancellationToken? cancellationToken = null)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request message is null.");

            var json = JsonConvert.SerializeObject(request);
            var typedRequest = JsonConvert.DeserializeObject<TRequest>(json);

            if (typedRequest == null)
                throw new RequestDeserializationException(typeof(TRequest), json);

            await Handle(typedRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling request of type {RequestType}.", typeof(TRequest).FullName);
            throw new RequestHandlingException(typeof(TRequest), ex);
        }
    }

    public abstract Task Handle(TRequest request, CancellationToken? cancellationToken);
}


public abstract class RequestHandler<TRequest,TResposne> : IRequestHandler<TRequest, TResposne> where TRequest : IRequest<TResposne>
{
    public string RequestName { get; } 
    protected RequestHandler()
    {
        RequestName = typeof(TRequest).Name; 
    }

    public Task<TResposne> HandleRequest(object request, LightMediatorOptions mediatorOptions, CancellationToken? cancellationToken)
    {
        // FIXME: change the mapper
        var json = JsonConvert.SerializeObject(request);
        var response = JsonConvert.DeserializeObject<TRequest>(json)!;
        return Handle(response, cancellationToken); 
    }
    public abstract Task<TResposne> Handle(TRequest request, CancellationToken? cancellationToken);

}