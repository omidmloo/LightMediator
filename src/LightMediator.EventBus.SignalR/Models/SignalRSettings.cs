namespace LightMediator.EventBus.SignalR.Models;

public class SignalRSettings
{
    public SignalRSettings()
    {
    }

    public string ServerAddress { get; set; } = null!; 
    public SignalRSettings(string serverAddress)
    {
        ServerAddress = serverAddress; 
    }


}
