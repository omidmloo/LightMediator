namespace LightMediator;
internal class Mediator : IMediator
{
    private readonly ILogger<Mediator> _logger;
    internal readonly LightMediatorOptions _mediatorOptions;
    private readonly IServiceProvider _serviceProvider;

    public IEnumerable<INotificationHandler> _notificationHandlers { get; }

    public Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger, LightMediatorOptions mediatorOptions)
    {
        _serviceProvider = serviceProvider;
        _notificationHandlers = serviceProvider.GetServices<INotificationHandler>();
        _logger = logger;
        _mediatorOptions = mediatorOptions;
    }

    public async Task Publish(INotification notification, CancellationToken? cancellationToken = null, bool waitAllToPublish = false)
    {
        var eventName = _mediatorOptions.IgnoreNamespaceInAssemblies
            ? notification.GetType().Name
            : notification.GetType().FullName;

        var matchingHandlers = _notificationHandlers
            .Where(c => c.NotificationName.Equals(eventName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var tasks = new List<Task>();

        foreach (var handler in matchingHandlers)
        {
            try
            {
                tasks.Add(handler.HandleNotification(notification, _mediatorOptions, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle notification with handler {Handler}", handler.GetType().Name);
            }
        }

        if (waitAllToPublish)
        {
            await Task.WhenAll(tasks);
        }
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken? cancellationToken = null) where TRequest : class, IRequest
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestName = GetRequestName(request.GetType());

        using var scope = _serviceProvider.CreateScope();

        foreach (var assembly in _mediatorOptions.Assemblies)
        {
            var requestTypes = assembly.GetTypes()
                .Where(type => IsMatchingType(type, requestName))
                .ToList();

            foreach (var requestType in requestTypes)
            {
                var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

                var handler = scope.ServiceProvider.GetService(handlerType);
                if (handler == null)
                    continue;

                var method = handlerType.GetMethod("HandleRequest");
                if (method == null)
                    throw new InvalidHandlerInterfaceException(handlerType);

                try
                {
                    var task = (Task)method.Invoke(handler, new object[] { request, _mediatorOptions, cancellationToken ?? CancellationToken.None })!;
                    await task;
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception invoking handler {Handler}", handlerType.Name);
                    throw new RequestInvocationException(handlerType, ex);
                }
            }
        }

        throw new HandlerNotFoundException(requestName);
    }

    public async Task<TResponse?> Send<TResponse>(IRequest<TResponse> request, CancellationToken? cancellationToken = null)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestName = GetRequestName(request.GetType());

        using var scope = _serviceProvider.CreateScope();

        foreach (var assembly in _mediatorOptions.Assemblies)
        {
            var requestTypes = assembly.GetTypes()
                .Where(type => IsMatchingType(type, requestName))
                .ToList();

            foreach (var requestType in requestTypes)
            {
                var responseType = requestType.GetInterfaces().FirstOrDefault()?.GetGenericArguments().FirstOrDefault();
                if (responseType == null)
                    continue;

                var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
                var handler = scope.ServiceProvider.GetService(handlerType);
                if (handler == null)
                    continue;

                var method = handlerType.GetMethod("HandleRequest");
                if (method == null)
                    throw new InvalidHandlerInterfaceException(handlerType);

                try
                {
                    var task = (Task)method.Invoke(handler, new object[] { request, _mediatorOptions, cancellationToken ?? CancellationToken.None })!;
                    var result = await ConvertToGenericTask(task, responseType);

                    return JsonConvert.DeserializeObject<TResponse>(JsonConvert.SerializeObject(result));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception invoking handler {Handler}", handlerType.Name);
                    throw new RequestInvocationException(handlerType, ex);
                }
            }
        }

        throw new HandlerNotFoundException(requestName);
    }

    private static async Task<object> ConvertToGenericTask(Task task, Type responseType)
    {
        var genericTaskType = typeof(Task<>).MakeGenericType(responseType);
        var resultProperty = genericTaskType.GetProperty("Result");

        await task.ConfigureAwait(false);
        return resultProperty?.GetValue(task)!;
    }

    private string GetRequestName(Type type)
    {
        return _mediatorOptions.IgnoreNamespaceInAssemblies ? type.Name : type.FullName!;
    }

    private bool IsMatchingType(Type type, string requestName)
    {
        return !type.IsAbstract && !type.IsInterface &&
               (_mediatorOptions.IgnoreNamespaceInAssemblies && type.Name == requestName ||
                !_mediatorOptions.IgnoreNamespaceInAssemblies && type.FullName == requestName);
    }

    IServiceProvider IMediator.GetServiceProvider() => _serviceProvider;
}
