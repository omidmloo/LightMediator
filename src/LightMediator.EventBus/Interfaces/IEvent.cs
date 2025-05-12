namespace LightMediator.EventBus;

public interface IEvent
{
    public string TypeName { get; set; }
    public string JsonPayload { get; set; }
}
