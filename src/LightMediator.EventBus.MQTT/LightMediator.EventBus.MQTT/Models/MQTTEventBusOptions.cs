
namespace LightMediator.EventBus.MQTT;

public class MQTTEventBusOptions
{
    public Type? TokenHandlerType { get; set; }
    internal readonly List<string> ExtraTopics = new List<string>();
    public MQTTEventBusOptions SetTokenHandler<T>()
    {
        TokenHandlerType = typeof(T);
        return this;
    }

    public MQTTEventBusOptions AddTopics(params string[] topics)
    {
        foreach (var topic in topics)
        {
            ExtraTopics.Add(topic);
        }
        return this;
    }
}