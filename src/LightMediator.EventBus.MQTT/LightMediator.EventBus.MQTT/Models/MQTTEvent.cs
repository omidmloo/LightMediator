namespace LightMediator.EventBus.MQTT;

internal class MQTTEvent : IEvent
{

    public string TypeName { get; set; }
    public string AssemblyName { get; set; }
    public string JsonPayload { get; set; }
    public MQTTEvent(string typeName, string jsonPayload, string assemblyName)
    {
        TypeName = typeName;
        JsonPayload = jsonPayload;
        AssemblyName = assemblyName;
    }

}
