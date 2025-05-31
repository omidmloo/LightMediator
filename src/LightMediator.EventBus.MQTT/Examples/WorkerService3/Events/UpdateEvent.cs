using LightMediator;

namespace WorkerService3.Eventss;

public class UpdateEvent : INotification
{
    public UpdateEventEnum Action { get; set; }
}

public enum UpdateEventEnum
{
    RefreshToken = 1,
    DeviceAddded = 10,
    DeviceRemoved = 11
}