
namespace LightMediator.EventBus.MQTT;

public class MQTTSettings
{
    public string ServerAddress { get; set; } = null!;
    public string WssServerAddress { get; set; } = null!;
    public int PortNumber { get; set; } 
    public int WssPortNumber { get; set; } 
    public string? ClientId { get; set; } 
    public string? Username { get; set; } 
    public string? Password { get; set; }
    public bool UseWebSocket { get; set; } = false;
    public bool UseTls { get; set; } = false;
    public bool IsDefaultEventBus { get; set; } = true;
    public string LightmediatorTopicName { get; set; } = $"lightmediator-events";
    public bool SubscribeToUsernameTopic { get; set; }
    public bool SendTokenInPassword { get; set; } = true;
    public bool AutoReconenctOnDisconnection { get; set; } = true;
}
