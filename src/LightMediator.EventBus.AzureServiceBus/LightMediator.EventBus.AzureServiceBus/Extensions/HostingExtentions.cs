using LightMediator.EventBus.AzureServiceBus.Implementations;

namespace LightMediator.EventBus.AzureServiceBus;

public static class HostingExtentions
{
    public static LightMediatorEventBusOptions UseAzureServiceBus(
   this LightMediatorEventBusOptions serviceBusOptions,
   IConfiguration configuration,
   string azureServiceBusSectionName)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (string.IsNullOrWhiteSpace(azureServiceBusSectionName)) throw new ArgumentException("AzureServiceBus section name is required.");

        return UseAzureServiceBus(serviceBusOptions, configuration.GetSection(azureServiceBusSectionName));
    }

    public static LightMediatorEventBusOptions UseAzureServiceBus(
         this LightMediatorEventBusOptions serviceBusOptions,
     IConfigurationSection configurationSection)
    {
        if (configurationSection == null) throw new ArgumentNullException(nameof(configurationSection));

        AzureServiceBusSettings? settings = configurationSection.Get<AzureServiceBusSettings>();
        if (settings == null)
            throw new AzureServiceBusConfigurationException("AzureServiceBus settings are invalid or missing.");

        return UseAzureServiceBus(serviceBusOptions, settings);
    }

    public static LightMediatorEventBusOptions UseAzureServiceBus(
        this LightMediatorEventBusOptions serviceBusOptions,
        AzureServiceBusSettings options)
    {
        if (options == null)
            throw new AzureServiceBusConfigurationException("AzureServiceBus settings are invalid or missing.");

        serviceBusOptions.ServiceCollection.AddSingleton<ILightMediatorEventBus, AzureServiceBusEventBus<AzureServiceBusEvent>>();
        serviceBusOptions.ServiceCollection.AddMassTransit(cfg =>
        {
            cfg.AddConsumer<AzureServiceBusEventBus<AzureServiceBusEvent>>();

            cfg.UsingAzureServiceBus((context, cfgAzure) =>
            {
                cfgAzure.Host(options.ConnectionString);

                if (options.EnableRetry)
                {
                    cfgAzure.UseMessageRetry(r =>
                    {
                        r.Interval(options.RetryCount, TimeSpan.FromMilliseconds(options.RetryIntervalMs));
                    });
                }
                cfgAzure.ReceiveEndpoint($"lightmediator-{Assembly.GetEntryAssembly()!.GetName().Name}-queue", e =>
                {
                    e.ConfigureConsumer<AzureServiceBusEventBus<AzureServiceBusEvent>>(context);
                });
                if (!string.IsNullOrEmpty(options.TopicName))
                {
                    cfgAzure.Message<AzureServiceBusEvent>(x =>
                    {
                        x.SetEntityName(options.TopicName);
                    });
                }
            });
        });
        serviceBusOptions.ServiceCollection.AddScoped<IAzureServiceBusEventPublisher, AzureServiceBusEventPublisher>();
        return serviceBusOptions;
    }
}
