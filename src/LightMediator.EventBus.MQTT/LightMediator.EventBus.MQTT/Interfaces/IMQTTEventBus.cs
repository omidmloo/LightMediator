
namespace LightMediator.EventBus.MQTT;

public interface IMQTTEventBus: ILightMediatorEventBus
{
    Task PublishAsync(INotification notification,params string[] extraTopics);
}