namespace LightMediator.EventBus.MQTT;

internal enum EventBusConnectionState
{
    Connected,
    Disconnected,
    Reconncting,
    Connecting
}
