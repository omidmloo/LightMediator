namespace LightMediator.EventBus;
 
public class LightMediatorEventBusOptions
{
    public IServiceCollection ServiceCollection { get; set; }
    public LightMediatorEventBusOptions(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

}