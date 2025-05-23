using LightMediator;

namespace WorkerService3.Events;

internal class TestEvent : INotification
{
    public string MyProperty { get; set; }

}
