
namespace LightMediator.EventBus.SignalR;

public static class HostingExtensions
{
    private static string HubName = @"hubs/LightMediator";
    public static LightMediatorEventBusOptions UseSignalRService(this LightMediatorEventBusOptions serviceBusOptions, IConfiguration configuration, string signalRSectionName)
    {
        return UseSignalRService(serviceBusOptions, configuration.GetSection(signalRSectionName)); 
    }
    public static LightMediatorEventBusOptions UseSignalRService(this LightMediatorEventBusOptions serviceBusOptions, IConfigurationSection section)
    {
        SignalRSettings signalRSettings = section.Get<SignalRSettings>()!;
        return UseSignalRService(serviceBusOptions, signalRSettings);
    }
    public static LightMediatorEventBusOptions UseSignalRService(this LightMediatorEventBusOptions serviceBusOptions, string signalRServerAddress)
    {
        string hubUrl = $"{signalRServerAddress}{HubName}";
        ConfigureSignalRServices(serviceBusOptions.ServiceCollection, hubUrl);
        return serviceBusOptions;
    }
    public static LightMediatorEventBusOptions UseSignalRService(this LightMediatorEventBusOptions serviceBusOptions, SignalRSettings signalRSettings)
    {
        string hubUrl = $"{signalRSettings.ServerAddress}{HubName}";
        ConfigureSignalRServices(serviceBusOptions.ServiceCollection, hubUrl);
        return serviceBusOptions;
    }
    private static void ConfigureSignalRServices(IServiceCollection services, string hubUrl)
    {
        services.AddSingleton(provider =>
        {
            return new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.HttpMessageHandlerFactory = inner =>
                    {
                        if (inner is HttpClientHandler clientHandler)
                        {
                            // WARNING: this will accept ANY certificate!
                            clientHandler.ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                        }
                        return inner;
                    };
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