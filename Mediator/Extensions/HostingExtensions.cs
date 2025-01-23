using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AppMediator;

public static class HostingExtensions
{
    public static IServiceCollection AddAppMediator(this IServiceCollection services, Action<AppMediatorOptions> configureOptions)
    {
        var options = new AppMediatorOptions();
        configureOptions?.Invoke(options);

        services.AddSingleton<AppMediatorOptions>(options);
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
