
namespace LightMediator.EventBus.SignalR;

public static class HostingExtensions
{
    private const string HubName = "hubs/LightMediator";

    public static LightMediatorEventBusOptions UseSignalRService(
        this LightMediatorEventBusOptions serviceBusOptions,
        IConfiguration configuration,
        string signalRSectionName,
        bool ignoreUntrustedCertificate = false)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (string.IsNullOrWhiteSpace(signalRSectionName)) throw new ArgumentException("SignalR section name is required.");

        return UseSignalRService(serviceBusOptions, configuration.GetSection(signalRSectionName), ignoreUntrustedCertificate);
    }

    public static LightMediatorEventBusOptions UseSignalRService(
        this LightMediatorEventBusOptions serviceBusOptions,
        IConfigurationSection section,
        bool ignoreUntrustedCertificate = false)
    {
        if (section == null) throw new ArgumentNullException(nameof(section));

        SignalRSettings? signalRSettings = section.Get<SignalRSettings>();
        if (signalRSettings == null || string.IsNullOrWhiteSpace(signalRSettings.ServerAddress))
            throw new SignalRConfigurationException("SignalR settings are invalid or missing ServerAddress.");

        return UseSignalRService(serviceBusOptions, signalRSettings, ignoreUntrustedCertificate);
    }

    public static LightMediatorEventBusOptions UseSignalRService(
        this LightMediatorEventBusOptions serviceBusOptions,
        string signalRServerAddress,
        bool ignoreUntrustedCertificate = false)
    {
        if (string.IsNullOrWhiteSpace(signalRServerAddress))
            throw new ArgumentException("SignalR server address must not be null or empty.", nameof(signalRServerAddress));

        string hubUrl = $"{signalRServerAddress.TrimEnd('/')}/{HubName}";
        ConfigureSignalRServices(serviceBusOptions.ServiceCollection, hubUrl, ignoreUntrustedCertificate);
        return serviceBusOptions;
    }

    public static LightMediatorEventBusOptions UseSignalRService(
        this LightMediatorEventBusOptions serviceBusOptions,
        SignalRSettings signalRSettings,
        bool ignoreUntrustedCertificate = false)
    {
        if (signalRSettings == null || string.IsNullOrWhiteSpace(signalRSettings.ServerAddress))
            throw new SignalRConfigurationException("SignalR settings are null or missing ServerAddress.");

        string hubUrl = $"{signalRSettings.ServerAddress.TrimEnd('/')}/{HubName}";
        ConfigureSignalRServices(serviceBusOptions.ServiceCollection, hubUrl, ignoreUntrustedCertificate);
        return serviceBusOptions;
    }

    private static void ConfigureSignalRServices(IServiceCollection services, string hubUrl, bool ignoreUntrustedCertificate = false)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(hubUrl)) throw new ArgumentException("Hub URL must not be null or empty.", nameof(hubUrl));

        try
        {
            services.AddSingleton(provider =>
            {
                return new HubConnectionBuilder()
                    .WithUrl(hubUrl, options =>
                    {
                        if (ignoreUntrustedCertificate)
                        {
                            options.HttpMessageHandlerFactory = inner =>
                            {
                                if (inner is HttpClientHandler clientHandler)
                                {
                                    clientHandler.ServerCertificateCustomValidationCallback =
                                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                                }
                                return inner;
                            };
                        }
                    })
                    .WithAutomaticReconnect()
                    .Build();
            });

            services.AddSingleton<SignalREventBus>();
            services.AddSingleton<ILightMediatorEventBus>(sp => sp.GetRequiredService<SignalREventBus>());
            services.AddHostedService(sp => sp.GetRequiredService<SignalREventBus>());
        }
        catch (Exception ex)
        {
            throw new SignalRConfigurationException("Failed to configure SignalR services.", ex);
        }
    }

    public static void ConfigureLightMediatorSignalR(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapHub<LightMediatorHub>(HubName);
    }
}
