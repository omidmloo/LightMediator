﻿
namespace LightMediator;

public abstract class RequestHandler<TRequest> : IRequestHandler<TRequest> where TRequest : class, IRequest
{
    public string RequestName { get; }
    private readonly LightMediatorOptions _mediatorOptions;
    protected RequestHandler(LightMediatorOptions mediatorOptions)
    {
        RequestName = typeof(TRequest).Name;
        _mediatorOptions = mediatorOptions;
    }

    public Task HandleRequest(object request, CancellationToken? cancellationToken)
    {
        var json = JsonConvert.SerializeObject(request);
        var sourceFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        var targetFields = Activator.CreateInstance(typeof(TRequest));
        var targetProperties = typeof(TRequest).GetProperties();

        if ((sourceFields == null ||
            sourceFields.Count() == 0))
        {
            throw new ArgumentNullException($"Source request has not property");
        }


        foreach (var property in targetProperties)
        {
            if (sourceFields!.ContainsKey(property.Name) && property.CanWrite)
            {
                var value = sourceFields[property.Name];
                property.SetValue(targetFields, Convert.ChangeType(value, property.PropertyType));
            }
            else if (!_mediatorOptions.IgnoreNotificationDifferences)
            {
                throw new InvalidCastException($"Cannot cast {RequestName}");
            }
        }
        return Handle((TRequest)targetFields!, cancellationToken);
    }
    public abstract Task Handle(TRequest request, CancellationToken? cancellationToken);

}


public abstract class RequestHandler<TRequest,TResposne> : IRequestHandler<TRequest, TResposne> where TRequest : IRequest<TResposne>
{
    public string RequestName { get; }
    private readonly LightMediatorOptions _mediatorOptions;
    protected RequestHandler(LightMediatorOptions mediatorOptions)
    {
        RequestName = typeof(TRequest).Name;
        _mediatorOptions = mediatorOptions;
    }

    public Task<TResposne> HandleRequest(object request, CancellationToken? cancellationToken)
    {
        var json = JsonConvert.SerializeObject(request);
        var sourceFields = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        var targetFields = Activator.CreateInstance(typeof(TRequest));
        var targetProperties = typeof(TRequest).GetProperties();

        if ((sourceFields == null ||
            sourceFields.Count() == 0))
        {
            throw new ArgumentNullException($"Source request has not property");
        }


        foreach (var property in targetProperties)
        {
            if (sourceFields!.ContainsKey(property.Name) && property.CanWrite)
            {
                var value = sourceFields[property.Name];
                property.SetValue(targetFields, Convert.ChangeType(value, property.PropertyType));
            }
            else if (!_mediatorOptions.IgnoreNotificationDifferences)
            {
                throw new InvalidCastException($"Cannot cast {RequestName}");
            }
        }
        return Handle((TRequest)targetFields!, cancellationToken);
    }
    public abstract Task<TResposne> Handle(TRequest request, CancellationToken? cancellationToken);

}