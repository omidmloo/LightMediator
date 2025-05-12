
namespace LightMediator.EventBus.SignalR;

public static class HostingExtensions
{
    private static string HubName = @"hubs/LightMediator";
    public static LightMediatorEventBusOptions UseSignalRService(this LightMediatorEventBusOptions serviceBusOptions, IConfiguration configuration, string signalRSectionName, bool ignoreUntrustedCertificate = false)
    {
        return UseSignalRService(serviceBusOptions, configuration.GetSection(signalRSectionName),ignoreUntrustedCertificate); 
    }
    public static LightMediatorEventBusOptions UseSignalRService(this LightMediatorEventBusOptions serviceBusOptions, IConfigurationSection section, bool ignoreUntrustedCertificate = false)
    {
        SignalRSettings signalRSettings = section.Get<SignalRSettings>()!;
        return UseSignalRService(serviceBusOptions, signalRSettings, ignoreUntrustedCertificate);
    }
    public static LightMediatorEventBusOptions UseSignalRService(this LightMediatorEventBusOptions serviceBusOptions, string signalRServerAddress, bool ignoreUntrustedCertificate = false)
    {
        string hubUrl = $"{signalRServerAddress}{HubName}";
        ConfigureSignalRServices(serviceBusOptions.ServiceCollection, hubUrl, ignoreUntrustedCertificate);
        return serviceBusOptions;
    }
    public static LightMediatorEventBusOptions UseSignalRService(this LightMediatorEventBusOptions serviceBusOptions, SignalRSettings signalRSettings, bool ignoreUntrustedCertificate = false)
    {
        string hubUrl = $"{signalRSettings.ServerAddress}{HubName}";
        ConfigureSignalRServices(serviceBusOptions.ServiceCollection, hubUrl, ignoreUntrustedCertificate);
        return serviceBusOptions;
    }
    private static void ConfigureSignalRServices(IServiceCollection services, string hubUrl,bool ignoreUntrustedCertificate = false)
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

    public static void ConfigureLightMediatorSignalR(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapHub<LightMediatorHub>(HubName);
    }
}