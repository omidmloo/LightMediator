namespace LightMediator.EventBus.AzureServiceBus.Models;

internal class AzureServiceBusEvent : IEvent
{

    public string TypeName { get; set; }
    public string AssemblyName { get; set; }
    public string JsonPayload { get; set; }
    public AzureServiceBusEvent(string typeName, string jsonPayload, string assemblyName)
    {
        TypeName = typeName;
        JsonPayload = jsonPayload;
        AssemblyName = assemblyName;
    }

}
