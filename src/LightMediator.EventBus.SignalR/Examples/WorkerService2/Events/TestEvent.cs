using LightMediator;

namespace WorkerService2.Events;

internal class TestEvent : INotification
{
    public string MyProperty { get; set; }
}
