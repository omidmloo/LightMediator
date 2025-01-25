using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LightMediator;

public static class HostingExtensions
{
    public static IServiceCollection AddLightMediator(
        this IServiceCollection services, 
        Action<LightMediatorOptions> configureOptions,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        var options = new LightMediatorOptions();
        configureOptions?.Invoke(options);

        // Register the mediator
       
        if(serviceLifetime == ServiceLifetime.Scoped)
        {
            services.AddScoped(o =>
            {
                return options;
            });
            services.TryAddScoped<IMediator, Mediator>();
        }
        else
        {
            services.AddSingleton(options);
            services.TryAddSingleton<IMediator, Mediator>();
        }



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

                    if (serviceLifetime == ServiceLifetime.Scoped)
                    {
                        services.AddScoped(typeof(INotificationHandler), handlerType);
                    }
                    else
                    {
                        services.AddSingleton(typeof(INotificationHandler), handlerType);
                    }

                }
            }
        }

        return services;
    }
  
}
