using LightMediator;

namespace WorkerService1.Events;

internal class TestEvent:INotification
{
    public string MyProperty { get; set; }
}
