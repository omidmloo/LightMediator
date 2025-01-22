using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace AppMediator;

public static class HostingExtensions
{
    public static IServiceCollection AddAppMediator(this IServiceCollection services)
    {
        services.TryAddSingleton<IMediator, Mediator>(); 
        return services;
    } 
    public static IServiceCollection RegisterNotificationsFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        { 
            var handlerTypes = assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface)  
                .Where(type => type.BaseType != null
                               && type.BaseType.IsGenericType
                               && type.BaseType.GetGenericTypeDefinition() == typeof(NotificationHandler<>))  
                .ToList();

            foreach (var handlerType in handlerTypes)
            { 
                    services.AddSingleton(typeof(INotificationHandler), handlerType); 
            }
        }
        return services;
    }
}
