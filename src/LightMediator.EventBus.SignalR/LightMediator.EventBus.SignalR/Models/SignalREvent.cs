using System.Reflection;

namespace LightMediator.EventBus.SignalR;

internal class SignalREvent : IEvent
{

    public string TypeName { get; set; }
    public string JsonPayload { get; set; } 
    public string AssemblyName { get; set; }
    public SignalREvent(string typeName, string jsonPayload, string assemblyName)
    {
        TypeName = typeName;
        JsonPayload = jsonPayload;
        AssemblyName = assemblyName;
    }

}
