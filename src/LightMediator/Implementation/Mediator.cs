﻿using Microsoft.Extensions.Logging; 

namespace LightMediator;
internal class Mediator : IMediator
{
    private readonly ILogger<Mediator> _logger;
    internal readonly LightMediatorOptions _mediatorOptions;
    private readonly IServiceProvider _serviceProvider;

    public IEnumerable<INotificationHandler> _notificationHandlers { get; } = new List<INotificationHandler>();
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

        List<Task> tasks = new List<Task>();
        foreach (var handler in matchingHandlers)
        {
            tasks.Add(handler.HandleNotification(notification, _mediatorOptions, cancellationToken)); 
        }
        if (waitAllToPublish)
        {
            await Task.WhenAll(tasks);
        } 
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken? cancellationToken = null) where TRequest : class, IRequest
    {
        if (request == null)
            throw new ArgumentNullException("request is null");

        var requestName = _mediatorOptions.IgnoreNamespaceInAssemblies
                          ? request.GetType().Name
                          : request.GetType().FullName;

        List<Type> requestTypes = new List<Type>();

        foreach (var assembly in _mediatorOptions.Assemblies)
        {

            requestTypes.AddRange(assembly.GetTypes()
                .Where(type => !type.IsAbstract &&
                               !type.IsInterface &&
                               (_mediatorOptions.IgnoreNamespaceInAssemblies &&
                                    type.Name == requestName) ||
                                    (!_mediatorOptions.IgnoreNamespaceInAssemblies &&
                                    type.FullName == requestName))
                .ToList());
        }
        using var scope = _serviceProvider.CreateScope();
        foreach (Type requestType in requestTypes)
        {
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
            if (handlerType != null)
            {
                var handler = scope.ServiceProvider.GetService(handlerType);

                if (handler != null)
                {
                    var handleMethod = handlerType.GetMethod("HandleRequest"); 
                    var task = (Task)handleMethod!.Invoke(handler, new object[] { request, _mediatorOptions, CancellationToken.None })!;

                    await task; // Ensure async execution
                    return;
                }
            }
        }
    }

    public async Task<TResponse?> Send<TResponse>(IRequest<TResponse> request, CancellationToken? cancellationToken = null)
    {
        if (request == null)
            throw new ArgumentNullException("request is null");

        var requestName = _mediatorOptions.IgnoreNamespaceInAssemblies
       ? request.GetType().Name
       : request.GetType().FullName;

        List<Type> requestTypes = new List<Type>();

        foreach (var assembly in _mediatorOptions.Assemblies)
        {

            requestTypes.AddRange(assembly.GetTypes()
                .Where(type => !type.IsAbstract &&
                               !type.IsInterface &&
                               (_mediatorOptions.IgnoreNamespaceInAssemblies &&
                                    type.Name == requestName) ||
                                    (!_mediatorOptions.IgnoreNamespaceInAssemblies &&
                                    type.FullName == requestName))
                .ToList());
        }
        using var scope = _serviceProvider.CreateScope();
        foreach (Type requestType in requestTypes)
        {
            Type? responseType = requestType.GetInterfaces()!.FirstOrDefault()!.GetGenericArguments()!.FirstOrDefault();
            if (responseType == null)
                continue;
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            if (handlerType != null)
            {
                var handler = scope.ServiceProvider.GetService(handlerType);

                if (handler != null)
                {
                    var handleMethod = handlerType.GetMethod("HandleRequest"); 

                    var task = (Task)handleMethod!.Invoke(handler, new object[] { request, _mediatorOptions, CancellationToken.None })!;
                     
                    object result = await ConvertToGenericTask(task, responseType);

                    return JsonConvert.DeserializeObject<TResponse>(JsonConvert.SerializeObject(result));
               
                }
            }
        }
        return default;
    }
    static async Task<object> ConvertToGenericTask(Task task, Type responseType)
    { 
        var genericTaskType = typeof(Task<>).MakeGenericType(responseType);
        var resultProperty = genericTaskType.GetProperty("Result");
         
        await task.ConfigureAwait(false);
        return resultProperty!.GetValue(task)!;
    }

    IServiceProvider IMediator.GetServiceProvider()
    {
        return _serviceProvider;
    } 
}
