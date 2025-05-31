
namespace WorkerService3.Token;

public interface ITokenHanlder
{
    Func<string, Task>? OnTokenChanged { get; set; }

    Task TokenChanged();
    Task<string> GetToken();
}
public class TokenHandler : ITokenHanlder
{
    private readonly IConfiguration _configuration;
    public TokenHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Func<string, Task>? OnTokenChanged { get ; set; }

    public async Task<string> GetToken()
    {
        await Task.CompletedTask;
        return _configuration.GetSection("MQTTSettings:Password").Get<string>()!;
    }

    public async Task TokenChanged()
    {
       await OnTokenChanged.Invoke("");
    }
}