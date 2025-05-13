using LightMediator.Exceptions;

namespace LightMediator;

public static class HostingExtensions
{
    public static IHostBuilder AddLightMediator(
        this IHostBuilder hostBuilder,
        Action<LightMediatorOptions> configureOptions)
    {
        hostBuilder.ConfigureServices((context, collection) =>
        {
            collection.AddLightMediator(configureOptions, ServiceLifetime.Singleton);
        });
        return hostBuilder;
    }

    public static IServiceCollection AddLightMediator(
        this IServiceCollection services,
        Action<LightMediatorOptions> configureOptions,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        if (configureOptions == null)
            throw new LightMediatorOptionsException("configureOptions cannot be null.");

        var options = new LightMediatorOptions();
        configureOptions.Invoke(options);

        // Register core options and mediator
        if (serviceLifetime == ServiceLifetime.Scoped)
        {
            services.AddScoped(_ => options);
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
                try
                {
                    var notificationHandlerTypes = assembly.GetTypes()
                        .Where(type => !type.IsAbstract && !type.IsInterface)
                        .Where(type => type.BaseType != null &&
                                       type.BaseType.IsGenericType &&
                                       type.BaseType.GetGenericTypeDefinition() == typeof(NotificationHandler<>))
                        .ToList();

                    foreach (var handlerType in notificationHandlerTypes)
                    {
                        if (!typeof(INotificationHandler).IsAssignableFrom(handlerType))
                        {
                            throw new InvalidHandlerRegistrationException(handlerType, "Does not implement INotificationHandler.");
                        }

                        if (serviceLifetime == ServiceLifetime.Scoped)
                            services.AddScoped(typeof(INotificationHandler), handlerType);
                        else
                            services.AddSingleton(typeof(INotificationHandler), handlerType);
                    }
                }
                catch (Exception ex)
                {
                    throw new AssemblyScanningException(assembly, ex);
                }
            }
        }

        if (options.RegisterRequestsByAssembly && options.Assemblies?.Any() == true)
        {
            foreach (var assembly in options.Assemblies)
            {
                try
                {
                    var handlerTypes1 = assembly.GetTypes()
                        .Where(type => !type.IsAbstract && !type.IsInterface)
                        .SelectMany(type => type.GetInterfaces(), (type, iface) => new { Type = type, Interface = iface })
                        .Where(t => t.Interface.IsGenericType &&
                                    t.Interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

                    foreach (var handler in handlerTypes1)
                    {
                        if (serviceLifetime == ServiceLifetime.Scoped)
                            services.AddScoped(handler.Interface, handler.Type);
                        else
                            services.AddSingleton(handler.Interface, handler.Type);
                    }

                    var handlerTypes2 = assembly.GetTypes()
                        .Where(type => !type.IsAbstract && !type.IsInterface)
                        .SelectMany(type => type.GetInterfaces(), (type, iface) => new { Type = type, Interface = iface })
                        .Where(t => t.Interface.IsGenericType &&
                                    t.Interface.GetGenericTypeDefinition() == typeof(IRequestHandler<>));

                    foreach (var handler in handlerTypes2)
                    {
                        if (serviceLifetime == ServiceLifetime.Scoped)
                            services.AddScoped(handler.Interface, handler.Type);
                        else
                            services.AddSingleton(handler.Interface, handler.Type);
                    }
                }
                catch (Exception ex)
                {
                    throw new AssemblyScanningException(assembly, ex);
                }
            }
        }

        return services;
    }
}
