namespace LightMediator.EventBus.MQTT;

public class MQTTNotification
{
    public INotification Notification { get; set; } = null!;
    public List<string> ExtraTopics { get; set; } = new List<string>();
}
