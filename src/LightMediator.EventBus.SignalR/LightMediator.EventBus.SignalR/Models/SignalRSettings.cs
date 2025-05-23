namespace LightMediator.EventBus.SignalR.Models;

public class SignalRSettings
{
    public string ServerAddress { get; set; } = null!; 
    public string? AccessToken { get; set; }
    public SignalRSettings()
    {
    }

    public SignalRSettings(string serverAddress)
    {
        ServerAddress = serverAddress; 
    }


}
