using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace AppMediator;

public static class HostingExtensions
{
    public static IServiceCollection AddAppMediator(this IServiceCollection services, Action<AppMediatorOptions>? configureOptions = null)
    {
        var options = new AppMediatorOptions();
        configureOptions?.Invoke(options);

        // Register the mediator
        services.TryAddSingleton<IMediator, Mediator>();

        // Conditionally register notification handlers if assemblies are provided
        if (options.RegisterNotificationsByAssembly && options.Assemblies?.Any() == true)
        {
            foreach (var assembly in options.Assemblies)
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
        }

        return services;
    }
}
// Options class for configuring mediator services
public class AppMediatorOptions
{
    public bool RegisterNotificationsByAssembly { get; set; } = true;
    public Assembly[] Assemblies { get; set; } = Array.Empty<Assembly>();
}
