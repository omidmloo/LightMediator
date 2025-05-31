
namespace LightMediator.EventBus.MQTT;

public interface IMQTTTokenHandler
{
    Task<TokenInfo> GetTokenInfoAsync(Func<string, Task>? onTokenChanged = null); 
}
