using System.Reflection;

namespace LightMediator.EventBus.SignalR;

internal class SignalREvent : IEvent
{

    public string TypeName { get; set; }
    public string JsonPayload { get; set; }
    public SignalREvent(string typeName, string jsonPayload)
    {
        TypeName = typeName;
        JsonPayload = jsonPayload;
    }

}
