using Microsoft.Extensions.Configuration;
using System.Net.Sockets;

namespace LightMediator.EventBus;

public static class HostingExtensions
{
    public static LightMediatorEventBusOptions UseMQTT(
        this LightMediatorEventBusOptions serviceBusOptions,
        IConfiguration configuration,
        string mqttSectionName,
        Func<MQTTEventBusOptions, MQTTEventBusOptions>? configureOptions = null)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (string.IsNullOrWhiteSpace(mqttSectionName)) throw new ArgumentException("MQTT section name is required.");
        return serviceBusOptions.UseMQTT(configuration.GetSection(mqttSectionName), configureOptions);
    }

    public static LightMediatorEventBusOptions UseMQTT(
        this LightMediatorEventBusOptions serviceBusOptions,
        IConfigurationSection configurationSection,
        Func<MQTTEventBusOptions, MQTTEventBusOptions>? configureOptions = null)
    {
        if (configurationSection == null) throw new ArgumentNullException(nameof(configurationSection));

        MQTTSettings? settings = configurationSection.Get<MQTTSettings>();
        if (settings == null)
            throw new MQTTConfigurationException("MQTT settings are invalid or missing.");

        return serviceBusOptions.UseMQTT(settings, configureOptions);
    }

    public static LightMediatorEventBusOptions UseMQTT(
        this LightMediatorEventBusOptions serviceBusOptions,
        MQTTSettings settings,
        Func<MQTTEventBusOptions, MQTTEventBusOptions>? configureOptions = null)
    {
        if (settings == null)
            throw new MQTTConfigurationException("MQTT settings are invalid or missing.");

        if (string.IsNullOrEmpty(settings.ClientId) && string.IsNullOrEmpty(settings.Username) && string.IsNullOrEmpty(settings.Password))
            throw new MQTTConfigurationException("MQTT credentials or client id are invalid or missing.");

        var factory = new MqttClientFactory();
        var client = factory.CreateMqttClient();
        serviceBusOptions.ServiceCollection.AddSingleton(settings);
        serviceBusOptions.ServiceCollection.AddSingleton(client);
        serviceBusOptions.ServiceCollection.AddSingleton<MQTTEventBus>();
        serviceBusOptions.ServiceCollection.AddSingleton<IMQTTEventBus>(sp => sp.GetRequiredService<MQTTEventBus>());
        serviceBusOptions.ServiceCollection.AddHostedService(sp => sp.GetRequiredService<MQTTEventBus>());


        serviceBusOptions.ServiceCollection.AddSingleton((p) =>
        {  
            var options = new MQTTEventBusOptions();
            if (configureOptions != null)
            {
                options = configureOptions.Invoke(options);
            }
            return options;
        });
        return serviceBusOptions;
    }
}
