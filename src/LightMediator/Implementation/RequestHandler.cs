
namespace LightMediator;

public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest> where TRequest : class, IRequest
{
    public string RequestName { get; } 
    protected RequestHandler()
    {
        RequestName = typeof(TRequest).Name; 
    }

    public Task HandleRequest(object request, LightMediatorOptions mediatorOptions, CancellationToken? cancellationToken)
    {
        // FIXME: change the mapper
        var json = JsonConvert.SerializeObject(request);
        var response = JsonConvert.DeserializeObject<TRequest>(json);
        return Handle(response, cancellationToken);
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
        var response = JsonConvert.DeserializeObject<TRequest>(json);
        return Handle(response, cancellationToken); 
    }
    public abstract Task<TResposne> Handle(TRequest request, CancellationToken? cancellationToken);

}