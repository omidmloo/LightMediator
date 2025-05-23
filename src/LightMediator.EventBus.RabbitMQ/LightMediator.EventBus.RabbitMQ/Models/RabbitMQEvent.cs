namespace LightMediator.EventBus.RabbitMQ.Models;

internal class RabbitMQEvent : IEvent
{

    public string TypeName { get; set; }
    public string AssemblyName { get; set; }
    public string JsonPayload { get; set; }
    public RabbitMQEvent(string typeName, string jsonPayload, string assemblyName)
    {
        TypeName = typeName;
        JsonPayload = jsonPayload;
        AssemblyName = assemblyName;
    }

}
