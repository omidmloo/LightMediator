
using LightMediator.EventBus.MQTT;
using Microsoft.Extensions.Configuration;

namespace WorkerService3.Token;

public class MQTTTokenHandler : IMQTTTokenHandler
{
    private readonly ITokenHanlder _tokenHanlder;
    private string _cachedToken = string.Empty;
    private readonly IConfiguration _configuration;


    public MQTTTokenHandler(ITokenHanlder tokenHanlder, IConfiguration configuration)
    {
        _tokenHanlder = tokenHanlder;
        _configuration = configuration;
    }
    public async Task<TokenInfo> GetTokenInfoAsync(Func<string, Task>? onTokenChanged = null)
    {
        var newToken = await _tokenHanlder.GetToken();
        if (newToken != _cachedToken)
        {
            _cachedToken = newToken;
            _tokenHanlder.OnTokenChanged = onTokenChanged; 
        }
        return new TokenInfo(_configuration.GetSection("MQTTSettings:Username").Get<string>()!, newToken);
    }
}
