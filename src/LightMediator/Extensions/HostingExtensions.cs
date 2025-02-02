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

        if (serviceLifetime == ServiceLifetime.Scoped)
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


         
        if (options.RegisterNotificationsByAssembly && options.Assemblies?.Any() == true)
        {
            foreach (var assembly in options.Assemblies)
            {
                var notificationHandlerTypes = assembly.GetTypes()
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .Where(type => type.BaseType != null
                                   && type.BaseType.IsGenericType
                                   && type.BaseType.GetGenericTypeDefinition() == typeof(NotificationHandler<>))
                    .ToList();

                foreach (var handlerType in notificationHandlerTypes)
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

        if (options.RegisterRequestsByAssembly && options.Assemblies?.Any() == true)
        {
            foreach (var assembly in options.Assemblies)
            {
                var requestHandlerTypes =  assembly.GetTypes()
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .SelectMany(type => type.GetInterfaces(), (type, iface) => new { Type = type, Interface = iface })
                    .Where(t => t.Interface.IsGenericType &&
                                t.Interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));


                foreach (var handler in requestHandlerTypes)
                {
                    if (serviceLifetime == ServiceLifetime.Scoped)
                    {
                        services.AddSingleton(handler.Interface, handler.Type);
                    }
                    else
                    {
                        services.AddScoped(handler.Interface, handler.Type);
                    }
                }  

                var handlerTypes = assembly.GetTypes()
                    .Where(type => !type.IsAbstract && !type.IsInterface)
                    .SelectMany(type => type.GetInterfaces(), (type, iface) => new { Type = type, Interface = iface })
                    .Where(t => t.Interface.IsGenericType &&
                                t.Interface.GetGenericTypeDefinition() == typeof(IRequestHandler<>));


                foreach (var handler in handlerTypes)
                {
                    if (serviceLifetime == ServiceLifetime.Scoped)
                    {
                        services.AddSingleton(handler.Interface, handler.Type);
                    }
                    else
                    {
                        services.AddScoped(handler.Interface, handler.Type);
                    }
                }


            }
        }

        return services;
    }

}
