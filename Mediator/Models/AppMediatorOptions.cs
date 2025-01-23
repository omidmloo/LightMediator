using System.Reflection;

namespace AppMediator;

// Options class for configuring mediator services
public class AppMediatorOptions
{
    public bool IgnoreNamespaceInAssemblies { get; set; }
    public bool RegisterNotificationsByAssembly { get; set; } = true;
    public Assembly[] Assemblies { get; set; } = Array.Empty<Assembly>();
}
