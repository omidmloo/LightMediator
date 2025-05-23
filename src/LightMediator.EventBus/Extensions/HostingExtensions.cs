using Microsoft.Extensions.Options;

namespace LightMediator.EventBus;

public static class HostingExtensions
{
    public static LightMediatorEventBusOptions AddLightMediatorEventBus(this LightMediatorOptions options, IServiceCollection services)
    {
        return new LightMediatorEventBusOptions(services);
    }

}
